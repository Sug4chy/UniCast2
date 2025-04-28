using FastEndpoints;
using FluentValidation;
using UniCast.Application.InternalApi.Command.Students.CreateStudent;
using UniCast.Domain.Students.ValueObjects;

namespace UniCast.InternalApi.Endpoints.V1.Students;

public sealed class CreateStudentEndpoint : Ep.Req<CreateStudentRequest>.NoRes
{
    private readonly CreateStudentCommandHandler _handler;

    public CreateStudentEndpoint(CreateStudentCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/internal-api/v1/students");
        AllowAnonymous();
        Validator<CreateStudentRequestValidator>();
    }

    public override async Task HandleAsync(CreateStudentRequest req, CancellationToken ct)
    {
        string[] studentFullnameParts = req.FullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var result = await _handler.HandleAsync(new CreateStudentCommand(
            Id: req.Id,
            FullName: StudentFullName.From($"{studentFullnameParts[1]} {studentFullnameParts[0]}"),
            GroupName: AcademicGroupName.From(req.GroupName),
            Username: req.Username
        ), ct);

        if (result.IsSuccess)
        {
            await SendAsync(null, StatusCodes.Status201Created, ct);
            return;
        }

        await SendAsync(new { Error = result.Error.Message }, statusCode: (int)result.Error.Group, ct);
    }
}

public readonly record struct CreateStudentRequest(
    long Id,
    string FullName,
    string GroupName,
    string Username
);

public sealed class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
{
    public CreateStudentRequestValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        RuleFor(x => x.FullName).NotEmpty();
        RuleFor(x => x.GroupName).NotEmpty();
        RuleFor(x => x.Username).NotEmpty();
    }
}