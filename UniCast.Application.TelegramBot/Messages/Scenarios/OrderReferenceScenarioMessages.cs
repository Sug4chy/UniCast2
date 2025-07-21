namespace UniCast.Application.TelegramBot.Messages.Scenarios;

public static class OrderReferenceScenarioMessages
{
    public const string Introduction = "Вы выбрали функцию заказа академической справки, которая подтверждает факт " +
                                       "обучения в учебном заведении.";

    public const string EnterPatronymic = "Для заказа справки, пожалуйста, укажите Ваше отчество. Это необходимо для " +
                                          "точной идентификации.";

    public const string InvalidMessageFormat = "Кажется, я Вас не понимаю. Введите корректное значение.";

    public const string EnterGroupName = "Укажите Вашу учебную группу.";
    public const string InvalidGroupName = "Укажите корректную учебную группу";

    public const string EnterReferencesCount = "Сколько справок Вам необходимо заказать? Пожалуйста, укажите " +
                                               "количество числом без лишних символов.";

    public const string InvalidNumberFormat = "Пожалуйста, укажите корректное число.";

    public const string ChooseReferenceOrderPurpose = "Для дальнейшего оформления, пожалуйста, укажите цель " +
                                                      "получения справки.";

    public const string InvalidPurpose = "Пожалуйста, выберите цель заказа из предложенных.";

    public const string EnterYourOrderPurpose = "Укажите свою цель получения справки.";

    public const string ChooseReferenceObtainingMethod = "Последний шаг: укажите, пожалуйста, удобный для Вас способ " +
                                                         "получения справки. Вы можете забрать её лично в деканате " +
                                                         "или выбрать электронную версию, которую мы отправим " +
                                                         "электронную почту.";

    public const string SelfPickupObtainingMethod = "Заберу лично";
    public const string SendMeAnEmailObtainingMethod = "Пришлите по эл. почте";
    public const string EmailObtainingMethodTemplate = "Пришлите на почту {0}";
    public const string InvalidObtainingMethod = "Пожалуйста, выберите корректный способ получения";

    public const string EnterYourEmail = "Пожалуйста, введите Вашу электронную почту, на которую будет доставлена " +
                                         "справка.";

    public const string InvalidEmailFormat = "Введённый адрес электронной почты некорректный. Пожалуйста, проверьте " +
                                             "его и попробуйте снова.";

    public const string FinalOrderVersionMessageTemplate = """
                                                           Давайте проверим, всё ли верно:

                                                           Ваше ФИО - {0}
                                                           Ваша группа - {1}
                                                           Количество справок - {2}
                                                           Цель заказа - {3}
                                                           Способ получения - {4}

                                                           Верно?
                                                           """;

    public const string InvalidIsFinalVersionRightAnswer = "Пожалуйста, выберите предложенный вариант ответа.";
    public const string OkLetsStartAgain = "Хорошо, давайте начнем оформление заново.";

    public const string MessageToMethodistTemplate = """
                                                     Здравствуйте, хочу заказать справку.

                                                     ФИО - {0}
                                                     Группа - {1}
                                                     Количество справок - {2}
                                                     Цель заказа - {3}
                                                     Способ получения - {4}
                                                     """;

    public const string Completed = "Справка была успешно заказана, ваше сообщение отправлено методисту";
}