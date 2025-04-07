namespace UniCast.Application.TelegramBot.Messages.Scenarios;

public static class RegistrationScenarioMessages
{
    public const string Greeting = "Здравствуйте! Я - бот, цель жизни которого, это передавать студентам информацию. " +
                                   "Давайте начнём знакомиться!";

    public const string EnterUsername = "Введите свой login от факультетского Moodle";

    public const string PleaseEnterUsername = "Пожалуйста, введите свой login";

    public const string CantRecognizeUserByUsername =
        "Я не узнаю этот login. Возможно, вы ввели его с ошибкой. Пожалуйста, повторите ввод";

    public const string ProbablyRecognizeUser = "Кажется, я узнал тебя! Ты же {0}, верно?";

    public const string PleaseReenterUsername = "Извините за эту промашку. Пожалуйста, введите свой login ещё раз";

    public const string EnterPassword = "Введите свой пароль от Moodle. Он не будет сохранён, и нужен для того, " +
                                        "чтобы методисты видели ваши сообщения именно от вашего имени";

    public const string PleaseEnterPassword = "Пожалуйста, введите свой пароль";

    public const string RegistrationCompleted = "{0}, поздравляем вас с успешным завершением регистрации!";
}