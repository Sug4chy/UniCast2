namespace UniCast.Application.TelegramBot.Scenarios.OrderReference;

public enum OrderReferenceState
{
    Started = 0,
    AskingForPatronymic = 1,
    AskingForGroup = 2,
    AskingForReferencesCount = 3,
    AskingForReferenceOrderPurpose = 4,
    AskingForReferenceObtainingMethod = 5,
    ShowingReferenceFinalVersion = 6,
    Completed = 7
}