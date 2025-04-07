using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using UniCast.Application.Abstractions.Persistence;
using UniCast.Application.TelegramBot.Scenarios.Registration.States;
using UniCast.Application.TelegramBot.Utlis;
using UniCast.Domain.Telegram.Entities;
using UniCast.Domain.Telegram.ValueObjects.Enums;

namespace UniCast.Application.TelegramBot.Scenarios.Registration;

public sealed class RegistrationScenarioExecutor : IScenarioExecutor<IRegistrationState>
{
    private readonly IDataContext _dataContext;
    private readonly IServiceProvider _serviceProvider;

    public RegistrationScenarioExecutor(
        IDataContext dataContext,
        IServiceProvider serviceProvider)
    {
        _dataContext = dataContext;
        _serviceProvider = serviceProvider;
    }

    public Scenario Scenario => Scenario.Registration;

    public async Task ChangeStateAsync(
        PrivateTelegramChat chat,
        IRegistrationState newState,
        Update update,
        CancellationToken ct = default)
    {
        chat.CurrentState = GetState(newState);
        await _dataContext.SaveChangesAsync(ct);
        await newState.OnStateChangedAsync(chat, update, ct);
    }

    public int GetState(IRegistrationState state)
        => (int)(state switch
        {
            RegistrationStartedState 
                => RegistrationScenarioState.Started,
            RegistrationWaitingForMoodleUsernameEnteredState 
                => RegistrationScenarioState.WaitingForMoodleUsernameEntered,
            RegistrationMoodleUsernameEnteredState 
                => RegistrationScenarioState.MoodleUsernameEntered,
            RegistrationStudentRecognizedState 
                => RegistrationScenarioState.StudentRecognized,
            RegistrationWaitingForMoodlePasswordEnteredState 
                => RegistrationScenarioState.WaitingForMoodlePasswordEntered,
            RegistrationCompletedState 
                => RegistrationScenarioState.Completed,
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        });

    public IRegistrationState GetState(int state)
        => Enum.Parse<RegistrationScenarioState>(state.ToString()) switch
        {
            RegistrationScenarioState.Started =>
                new RegistrationStartedState(this, _serviceProvider),
            RegistrationScenarioState.WaitingForMoodleUsernameEntered =>
                new RegistrationWaitingForMoodleUsernameEnteredState(this, _serviceProvider),
            RegistrationScenarioState.MoodleUsernameEntered =>
                new RegistrationMoodleUsernameEnteredState(this, _serviceProvider),
            RegistrationScenarioState.StudentRecognized =>
                new RegistrationStudentRecognizedState(this, _serviceProvider),
            RegistrationScenarioState.WaitingForMoodlePasswordEntered =>
                new RegistrationWaitingForMoodlePasswordEnteredState(this, _serviceProvider),
            RegistrationScenarioState.Completed =>
                new RegistrationCompletedState(this, _serviceProvider),
            _ => throw new ArgumentOutOfRangeException(nameof(state))
        };

    public async ValueTask<bool> CanStartScenarioAsync(Update update, CancellationToken ct = default)
    {
        long chatId = TelegramHelpers.GetChatId(update);

        return update.Type is UpdateType.Message &&
               update.Message!.Text is not null &&
               update.Message.Text == "/start"
               && await _dataContext.TelegramChats
                   .Cast<PrivateTelegramChat>()
                   .AnyAsync(x => x.ExtId == chatId &&
                                  x.StudentId == null, ct);
    }

    public async Task ClearScenarioAsync(PrivateTelegramChat chat, CancellationToken ct = default)
    {
        chat.CurrentScenario = null;
        chat.CurrentState = null;
        chat.CurrentScenarioArgs = [];

        await _dataContext.SaveChangesAsync(ct);
    }

    public Task StartScenarioAsync(PrivateTelegramChat chat, Update update, CancellationToken ct = default)
        => GetState((int)RegistrationScenarioState.Started)
            .OnStateChangedAsync(chat, update, ct);

    IState IScenarioExecutor.GetState(int state)
        => GetState(state);
}