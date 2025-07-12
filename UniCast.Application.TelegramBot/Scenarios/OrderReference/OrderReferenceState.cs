namespace UniCast.Application.TelegramBot.Scenarios.OrderReference;

public enum OrderReferenceState
{
    Started = 0,
    AskingForPatronymic = 1,
    AskingForGroup = 2,
    AskingForReferencesCount = 3,
    AskingForReferenceOrderPurpose = 4,
    OtherOrderPurposeSelected = 5,
    AskingForReferenceObtainingMethod = 6,
    ShowingReferenceFinalVersion = 7,
    Completed = 8
}