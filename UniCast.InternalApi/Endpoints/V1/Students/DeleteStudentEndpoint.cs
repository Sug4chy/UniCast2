using FastEndpoints;
using FluentValidation.Results;
using UniCast.Application.InternalApi.Command.Students.DeleteStudent;

namespace UniCast.InternalApi.Endpoints.V1.Students;

public sealed class DeleteStudentEndpoint : Ep.NoReq.NoRes
{
    private readonly DeleteStudentCommandHandler _handler;

    public DeleteStudentEndpoint(DeleteStudentCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Delete("/internal-api/v1/students/{id:long}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        long id = Route<long>("id");
        if (id <= 0)
        {
            AddError(new ValidationFailure("id", "id должно быть больше 0"));
            await SendErrorsAsync(cancellation: ct);
            return;
        }

        var result = await _handler.HandleAsync(new DeleteStudentCommand(id), ct);
        if (result.IsSuccess)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await SendAsync(new { Error = result.Error.Message }, statusCode: (int)result.Error.Group, ct);
    }
}