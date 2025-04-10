namespace UniCast.Application.TelegramBot.Messages.Scenarios;

public static class RefreshTokenScenarioMessages
{
    public const string PasswordHasExpired = "Время действия вашего пароля истекло. И раз уж мы его не сохраняем, " +
                                             "то просим вас ввести его ещё раз, чтобы вы могли продолжать " +
                                             "пользоваться ботом";

    public const string PleaseEnterPassword = "Пожалуйста, введите свой пароль";

    public const string NotAuthorized = "Не получилось авторизовать вас в системе Moodle. " +
                                        "Возможно, вы ввели пароль неверно, попробуйте ещё раз";
}