namespace UniCast.Application.TelegramBot.Messages.Scenarios;

// Текста сообщений будут изменены
public static class OrderReferenceScenarioMessages
{
    public const string Introduction = "Погнали";

    public const string EnterPatronymic = "Введите отчество пж";
    public const string InvalidMessageFormat = "Текстом ответь ёпта";

    public const string EnterGroupName = "Введите вашу группу пж";
    public const string InvalidGroupName = "Бля, нормально введи";

    public const string EnterReferencesCount = "Скок надо ёпта";
    public const string InvalidNumberFormat = "Чё с числом нах";

    public const string ChooseReferenceOrderPurpose = "Нахуя справка?";
    public const string TransportCardReferenceOrderPurpose = "Получение транспортной карты";
    public const string ParentsFaxDeductionReferenceOrderPurpose = "Налоговый вычет родителям";
    public const string OtherReferenceOrderPurpose = "Другое";
    public const string InvalidPurpose = "Сам понюхал, чё пукнул?";

    public const string EnterYourOrderPurpose = "Ну и нахуя?";

    public const string ChooseReferenceObtainingMethod = "Как забирать будешь, мудила?";
    public const string SelfPickupObtainingMethod = "Заберу лично";
    public const string SendMeAnEmailObtainingMethod = "Пришлите по эл. почте";
    public const string InvalidObtainingMethod = "Выбери нормально бля";

    public const string FinalOrderVersionMessageTemplate = """
                                                            Давайте проверим, всё ли верно:

                                                            Ваше ФИО - {0}
                                                            Ваша группа - {1}
                                                            Количество справок - {2}
                                                            Цель заказа - {3}
                                                            Способ получения - {4}

                                                            Верно?
                                                            """;

    public const string InvalidIsFinalVersionRightAnswer = "Сука ну выбери нормально блять";
    public const string OkLetsStartAgain = "Хорошо, го по-новой";
}