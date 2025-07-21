namespace UniCast.Application.TelegramBot.Messages.Scenarios;

public static class RefreshTokenScenarioMessages
{
    public const string PasswordHasExpired = "Время действия пароля истекло. Мы не сохраняем пароли, поэтому нужно " +
                                             "ввести его заново, чтобы продолжить пользоваться ботом.";

    public const string PleaseEnterPassword = "Пожалуйста, введите Ваш пароль.";

    public const string NotAuthorized = "Не получилось авторизоваться в системе Moodle. Возможно, введённый пароль " +
                                        "неверный, попробуйте ещё раз.";
}