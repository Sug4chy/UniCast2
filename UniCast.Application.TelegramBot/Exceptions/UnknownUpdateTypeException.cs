namespace UniCast.Application.TelegramBot.Exceptions;

public sealed class UnknownUpdateTypeException(string message) : Exception(message)
{
    public UnknownUpdateTypeException() : this("Unknown type of incomed Update")
    {
    }
}