using System.Text.Json;

namespace UniCast.Infrastructure.Moodle.Errors;

public readonly struct AccessError
{
    public string Exception { get; init; }
    public string ErrorCode { get; init; }
    public string Message { get; init; }
    public string DebugInfo { get; init; }

    private AccessError(string exception, string errorCode, string message, string debugInfo)
    {
        Exception = exception;
        ErrorCode = errorCode;
        Message = message;
        DebugInfo = debugInfo;
    }

    public static bool TryParse(string value, out AccessError error)
    {
        try
        {
            error = JsonSerializer.Deserialize<AccessError>(value);
            return true;
        }
        catch
        {
            error = default;
            return false;
        }
    }
}