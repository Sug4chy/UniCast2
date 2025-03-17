using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Repositories;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Handlers;
using UniCast.Application.TelegramBot.Scenarios;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Application.TelegramBot;

public sealed class UpdateDispatcher
{
    private readonly IEnumerable<IUpdateHandler> _handlers;
    private readonly IEnumerable<IScenarioExecutor> _scenarioExecutors;
    private readonly ITelegramChatRepository _telegramChatRepository;
    private readonly ILogger<UpdateDispatcher> _logger;
    private readonly ITelegramMessageSender _telegramMessageSender;

    public UpdateDispatcher(
        IEnumerable<IUpdateHandler> handlers,
        ITelegramChatRepository telegramChatRepository,
        IEnumerable<IScenarioExecutor> scenarioExecutors,
        ILogger<UpdateDispatcher> logger,
        ITelegramMessageSender telegramMessageSender)
    {
        _handlers = handlers;
        _telegramChatRepository = telegramChatRepository;
        _scenarioExecutors = scenarioExecutors;
        _logger = logger;
        _telegramMessageSender = telegramMessageSender;
    }

    public async Task DispatchAsync(Update update, CancellationToken ct = default)
    {
        try
        {
            foreach (var handler in _handlers)
            {
                if (!await handler.CanHandleAsync(update, ct))
                {
                    continue;
                }

                await handler.HandleAsync(update, ct);
                return;
            }

            var chat = await _telegramChatRepository.GetPrivateChatByExtIdAsync(GetChatId(update), ct)
                       ?? await CreateFromUpdateAsync(update, ct);

            if (chat.CurrentScenario.HasNoValue)
            {
                var scenario = _scenarioExecutors.FirstOrDefault(s => s.CanStartScenario(update));
                chat.CurrentScenario = scenario?.Scenario ?? Maybe<Scenario>.None;
                await (scenario?.StartScenarioAsync(chat, update, ct) ?? Task.CompletedTask);
            }
            else
            {
                var scenario = _scenarioExecutors.FirstOrDefault(s => s.Scenario == chat.CurrentScenario);
                await (scenario?.GetState(chat.CurrentState.Value)?
                    .HandleUserInputAsync(chat, update, ct) ?? Task.CompletedTask);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception message: '{Message}'", ex.Message);
            await _telegramMessageSender.SendMessageAsync(
                chatId: GetChatId(update),
                text: "Кажется, что-то пошло не так...",
                ct: ct);
        }
    }

    private static long GetChatId(Update update)
        => update.Type switch
        {
            UpdateType.Message => update.Message!.Chat.Id,
            UpdateType.CallbackQuery => update.CallbackQuery!.Message!.Chat.Id,
            _ => throw new ArgumentOutOfRangeException(nameof(update))
        };

    private async Task<PrivateTelegramChat> CreateFromUpdateAsync(Update update, CancellationToken ct = default)
    {
        var chat = PrivateTelegramChat.CreateNew(
            IdOf<TelegramChat>.New(),
            update.Message!.Chat.Username!,
            update.Message!.Chat.Id
        );

        await _telegramChatRepository.AddPrivateAsync(chat, ct);

        return chat;
    }
}