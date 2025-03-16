using CSharpFunctionalExtensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Repositories;
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

    public UpdateDispatcher(
        IEnumerable<IUpdateHandler> handlers, 
        ITelegramChatRepository telegramChatRepository, 
        IEnumerable<IScenarioExecutor> scenarioExecutors)
    {
        _handlers = handlers;
        _telegramChatRepository = telegramChatRepository;
        _scenarioExecutors = scenarioExecutors;
    }

    public async Task DispatchAsync(Update update, CancellationToken ct = default)
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

        var scenario = _scenarioExecutors.FirstOrDefault(s => s.Scenario == chat.CurrentScenario);
        if (chat.CurrentScenario.HasNoValue)
        {
            chat.CurrentScenario = scenario?.Scenario ?? Maybe<Scenario>.None;
            await (scenario?.StartScenarioAsync(chat, update, ct) ?? Task.CompletedTask);
        }
        else
        {
            await (scenario?.GetState(chat.CurrentState.Value)?
                .HandleUserInputAsync(chat, update, ct) ?? Task.CompletedTask);
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
            update.Message!.Chat.Title!,
            update.Message!.Chat.Id
        );

        await _telegramChatRepository.AddPrivateAsync(chat, ct);

        return chat;
    }
}