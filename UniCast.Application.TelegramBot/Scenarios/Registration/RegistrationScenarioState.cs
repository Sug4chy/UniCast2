namespace UniCast.Application.TelegramBot.Scenarios.Registration;

public enum RegistrationScenarioState
{
    Started = 0,
    WaitingForMoodleUsernameEntered = 1,
    StudentRecognized = 2,
    WaitingForMoodlePasswordEntered = 3,
    Completed = 4
}