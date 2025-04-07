namespace UniCast.Application.Abstractions.Moodle;

public interface IMoodleClient
{
    Task<CSharpFunctionalExtensions.Result<string>> LoginAsync(
        string username,
        string password,
        CancellationToken ct = default);
}