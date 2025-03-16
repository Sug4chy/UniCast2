namespace UniCast.Application.TelegramBot.Scenarios.Registration;

public enum RegistrationScenarioState
{
    Started = 0,
    WaitingForFullNameEntered = 1,
    WaitingForGroupNameEntered = 2,
    Completed = 3
}