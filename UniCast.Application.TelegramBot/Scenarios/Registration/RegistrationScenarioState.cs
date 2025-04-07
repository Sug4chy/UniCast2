namespace UniCast.Application.TelegramBot.Scenarios.Registration;

public enum RegistrationScenarioState
{
    Started = 0,
    WaitingForMoodleLoginEntered = 1,
    MoodleLoginEntered = 2,
    StudentRecognized = 3,
    WaitingForMoodlePasswordEntered = 4,
    Completed = 5
}