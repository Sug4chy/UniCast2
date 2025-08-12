using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.TelegramBot.Exceptions;
using UniCast.Application.TelegramBot.Handlers;
using UniCast.Application.TelegramBot.Scenarios;
using UniCast.Application.TelegramBot.Utils;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot;

public sealed class UpdateDispatcher
{
    private readonly IEnumerable<IUpdateHandler> _handlers;
    private readonly IEnumerable<IScenarioExecutor> _scenarioExecutors;
    private readonly IDataContext _dataContext;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly ILogger<UpdateDispatcher> _logger;

    public UpdateDispatcher(
        IEnumerable<IUpdateHandler> handlers,
        IEnumerable<IScenarioExecutor> scenarioExecutors,
        IDataContext dataContext,
        ITelegramMessageManager telegramMessageManager,
        ILogger<UpdateDispatcher> logger)
    {
        _handlers = handlers;
        _scenarioExecutors = scenarioExecutors;
        _dataContext = dataContext;
        _telegramMessageManager = telegramMessageManager;
        _logger = logger;
    }

    public async Task DispatchAsync(Update update, CancellationToken ct = default)
    {
        try
        {
            var handler = await GetHandlerForUpdateAsync(update, ct);
            if (handler is not null)
            {
                await handler.HandleAsync(update, ct);
                return;
            }

            var chat = await GetChatByExtIdAsync(TelegramHelpers.GetChatId(update), ct)
                       ?? await CreateFromUpdateAsync(update, ct);

            if (chat.CurrentScenario is null)
            {
                var scenario = await GetScenarioExecutorForUpdateAsync(update, ct);
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
        catch (ScenarioCancelledException ex)
        {
            _logger.LogInformation(ex, ex.Message);
        }
        catch (UnknownUpdateTypeException ex)
        {
            _logger.LogError(ex, "Exception message: '{Message}'", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception message: '{Message}'", ex.Message);
            await _telegramMessageManager.SendMessageAsync(
                chatId: TelegramHelpers.GetChatId(update),
                text: "Кажется, что-то пошло не так...",
                ct: ct
            );
        }
    }

    private async Task<IUpdateHandler?> GetHandlerForUpdateAsync(Update update, CancellationToken ct)
    {
        foreach (var handler in _handlers)
        {
            if (await handler.CanHandleAsync(update, ct))
            {
                return handler;
            }
        }

        return null;
    }

    private async Task<TelegramChat> CreateFromUpdateAsync(Update update, CancellationToken ct = default)
    {
        var chat = TelegramChat.CreateNew(
            IdOf<TelegramChat>.New(),
            update.Message!.Chat.Username!,
            update.Message!.Chat.Id
        );

        _dataContext.TelegramChats.Add(chat);
        await _dataContext.SaveChangesAsync(ct);

        return chat;
    }

    private Task<TelegramChat?> GetChatByExtIdAsync(long chatExtId, CancellationToken ct = default)
        => _dataContext.TelegramChats
            .FirstOrDefaultAsync(x => x.ExtId == chatExtId, ct);

    private async Task<IScenarioExecutor?> GetScenarioExecutorForUpdateAsync(
        Update update,
        CancellationToken ct = default)
    {
        foreach (var scenarioExecutor in _scenarioExecutors)
        {
            if (await scenarioExecutor.CanStartScenarioAsync(update, ct))
            {
                return scenarioExecutor;
            }
        }

        return null;
    }
}