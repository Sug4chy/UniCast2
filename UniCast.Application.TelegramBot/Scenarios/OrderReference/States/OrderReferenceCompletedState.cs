using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using UniCast.Application.Abstractions.Moodle;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.Abstractions.Telegram;
using UniCast.Application.Result;
using UniCast.Application.TelegramBot.Messages.Scenarios;
using UniCast.Application.TelegramBot.Scenarios.RefreshToken;
using UniCast.Domain.Telegram.Entities;

namespace UniCast.Application.TelegramBot.Scenarios.OrderReference.States;

public sealed class OrderReferenceCompletedState : IOrderReferenceState
{
    private readonly OrderReferenceScenarioExecutor _scenarioExecutor;
    private readonly ITelegramMessageManager _telegramMessageManager;
    private readonly IMoodleClient _moodleClient;
    private readonly RefreshTokenScenarioExecutor _refreshTokenScenarioExecutor;
    private readonly IDataContext _dataContext;

    public OrderReferenceCompletedState(
        OrderReferenceScenarioExecutor scenarioExecutor,
        IServiceProvider serviceProvider)
    {
        _scenarioExecutor = scenarioExecutor;
        _telegramMessageManager = serviceProvider.GetRequiredService<ITelegramMessageManager>();
        _moodleClient = serviceProvider.GetRequiredService<IMoodleClient>();
        _refreshTokenScenarioExecutor = serviceProvider.GetRequiredService<RefreshTokenScenarioExecutor>();
        _dataContext = serviceProvider.GetRequiredService<IDataContext>();
    }

    private static string BuildObtainingMethodString(Dictionary<string, string> scenarioArgs)
        => scenarioArgs.TryGetValue(OrderReferenceScenarioArgsKeys.Email, out string? value)
            ? $"Пришлите на почту {value}"
            : OrderReferenceScenarioMessages.SelfPickupObtainingMethod;

    public async Task OnStateChangedAsync(TelegramChat chat, Update update, CancellationToken ct = default)
    {
        var student = await _dataContext.Students
            .Include(x => x.MoodleAccount)
            .FirstAsync(x => x.Id == chat.StudentId, ct);
        string studentFullName =
            $"{student.FullName.ToString()} {chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.Patronymic]}";

        string messageToMethodist = string.Format(OrderReferenceScenarioMessages.MessageToMethodistTemplate,
            studentFullName,
            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.GroupName],
            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.ReferencesCount],
            chat.CurrentScenarioArgs[OrderReferenceScenarioArgsKeys.OrderPurpose],
            BuildObtainingMethodString(chat.CurrentScenarioArgs));

        var orderResult = await _moodleClient.SendMessageToIssuingMethodistAsync(
            senderToken: student.MoodleAccount!.CurrentToken!, 
            text: messageToMethodist, 
            ct: ct);

        if (orderResult.IsFailure)
        {
            if (orderResult.Error.Group is ErrorGroup.AccessError)
            {
                await _refreshTokenScenarioExecutor.StartScenarioAsync(
                    chat: chat,
                    update: update,
                    ct: ct);
            }
            else
            {
                await _telegramMessageManager.SendMessageAsync(
                    chatId: update.Message!.Chat.Id,
                    text: "Кажется, что-то пошло не так. Пожалуйста, повторите попытку позже",
                    ct: ct.IsCancellationRequested ? CancellationToken.None : ct);
            }

            return;
        }

        await _telegramMessageManager.SendMessageAsync(
            chatId: update.Message!.Chat.Id,
            text: "Справка была успешно заказана, ваше сообщение отправлено методисту",
            ct: ct);

        await _scenarioExecutor.ClearScenarioAsync(chat, ct);
    }

    public Task HandleUserInputAsync(TelegramChat chat, Update update, CancellationToken ct = default)
        => _scenarioExecutor.GetState((int)OrderReferenceState.ShowingReferenceFinalVersion)
            .HandleUserInputAsync(chat, update, ct);
}