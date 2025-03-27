using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Handlers;
using UniCast.Application.TelegramBot.Scenarios;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot;

public sealed class UpdateDispatcher
{
    private static readonly Exception ExceptionToIgnore = new();

    private readonly IEnumerable<IUpdateHandler> _handlers;
    private readonly IEnumerable<IScenarioExecutor> _scenarioExecutors;
    private readonly IDataContext _dataContext;
    private readonly ITelegramMessageSender _telegramMessageSender;
    private readonly ILogger<UpdateDispatcher> _logger;

    public UpdateDispatcher(
        IEnumerable<IUpdateHandler> handlers,
        IEnumerable<IScenarioExecutor> scenarioExecutors,
        IDataContext dataContext,
        ITelegramMessageSender telegramMessageSender,
        ILogger<UpdateDispatcher> logger)
    {
        _handlers = handlers;
        _scenarioExecutors = scenarioExecutors;
        _dataContext = dataContext;
        _telegramMessageSender = telegramMessageSender;
        _logger = logger;
    }

    public async Task DispatchAsync(Update update, CancellationToken ct = default)
    {
        try
        {
            // TODO распараллелить выбор нужного обработчика
            foreach (var handler in _handlers)
            {
                if (!await handler.CanHandleAsync(update, ct))
                {
                    continue;
                }

                await handler.HandleAsync(update, ct);
                return;
            }

            var chat = await GetPrivateChatByExtIdAsync(GetChatId(update), ct)
                       ?? await CreateFromUpdateAsync(update, ct);

            if (chat.CurrentScenario is null)
            {
                var scenario = _scenarioExecutors.FirstOrDefault(s => s.CanStartScenario(update));
                chat.CurrentScenario = scenario?.Scenario;
                await (scenario?.StartScenarioAsync(chat, update, ct) ?? Task.CompletedTask);
            }
            else
            {
                var scenario = _scenarioExecutors.FirstOrDefault(s => s.Scenario == chat.CurrentScenario);
                await (scenario?.GetState(chat.CurrentState!.Value)?
                    .HandleUserInputAsync(chat, update, ct) ?? Task.CompletedTask);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception message: '{Message}'", ex.Message);
            if (ex != ExceptionToIgnore)
            {
                await _telegramMessageSender.SendMessageAsync(
                    chatId: GetChatId(update),
                    text: "Кажется, что-то пошло не так...",
                    ct: ct
                );
            }
        }
    }

    private static long GetChatId(Update update)
        => update.Type switch
        {
            UpdateType.Message => update.Message!.Chat.Id,
            UpdateType.CallbackQuery => update.CallbackQuery!.Message!.Chat.Id,
            _ => throw ExceptionToIgnore
        };

    private async Task<PrivateTelegramChat> CreateFromUpdateAsync(Update update, CancellationToken ct = default)
    {
        var chat = PrivateTelegramChat.CreateNew(
            IdOf<TelegramChat>.New(),
            update.Message!.Chat.Username!,
            update.Message!.Chat.Id
        );

        _dataContext.TelegramChats.Add(chat);
        await _dataContext.SaveChangesAsync(ct);

        return chat;
    }

    private Task<PrivateTelegramChat?> GetPrivateChatByExtIdAsync(long chatExtId, CancellationToken ct = default)
        => _dataContext.TelegramChats
            .Cast<PrivateTelegramChat>()
            .SingleOrDefaultAsync(x => x.ExtId == chatExtId, ct);
}