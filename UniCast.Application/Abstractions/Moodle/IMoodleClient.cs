using CSharpFunctionalExtensions;
using UniCast.Application.Result;

namespace UniCast.Application.Abstractions.Moodle;

public interface IMoodleClient
{
    Task<Result<string>> LoginAsync(
        string username,
        string password,
        CancellationToken ct = default);

    Task<UnitResult<Error>> SendMessageAsync(
        string senderToken,
        int receiverExtId,
        string text,
        CancellationToken ct = default);
}