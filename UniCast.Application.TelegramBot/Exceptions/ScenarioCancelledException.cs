namespace UniCast.Application.TelegramBot.Exceptions;

public sealed class ScenarioCancelledException : Exception
{
    public ScenarioCancelledException(string scenarioName) : base($"Scenario {scenarioName} was cancelled by user.")
    {
    }
}