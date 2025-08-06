namespace UniCast.Application.TelegramBot.Messages.Scenarios;

public static class RegistrationScenarioMessages
{
    public const string Greeting = """
                                   Здравствуйте! Я Ваш университетский помощник.

                                   Я помогу Вам: 

                                   📢 Получать сообщения от деканата и методистов из Moodle с возможностью ответа на них

                                   📄 Заказать академические справки

                                   ❓ Найти ответы на самые частые вопросы

                                   Чтобы начать работу, нужно пройти авторизацию!
                                   """;

    public const string EnterUsername = "Пожалуйста, введите свой логин от факультетского Moodle, чтобы мы могли " +
                                        "подтвердить Вашу личность и приступить к работе!";

    public const string InvalidUsernameMessageFormat = "Пожалуйста, введите свой логин.";

    public const string CantRecognizeUserByUsername = "К сожалению, введённый логин не найден. Пожалуйста, проверьте " +
                                                      "правильность введённых данных и введите логин снова.";

    public const string ProbablyRecognizeUser = "Отлично! Кажется, я узнал Вас, Вы - {0}. Всё верно?";

    public const string PleaseReenterUsername = "Ой, что-то пошло не так при обработке Вашего логина. Нужно " +
                                                "попробовать еще раз! Пожалуйста, введите свой логин от Moodle снова.";

    public const string EnterPassword = "Теперь, пожалуйста, введите свой пароль от Moodle. Не волнуйтесь, он не " +
                                        "будет сохранен и нужен для того, чтобы методисты видели Ваше сообщения " +
                                        "именно от Вашего имени.";

    public const string PleaseEnterPassword = "Пожалуйста, введите свой пароль.";

    public const string RegistrationCompleted = "{0}, поздравляем с успешным завершением регистрации!";
}