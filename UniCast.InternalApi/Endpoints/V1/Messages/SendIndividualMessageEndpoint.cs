using FastEndpoints;
using FluentValidation;
using UniCast.Application.InternalApi.Command.Messages.SendIndividualMessage;
using UniCast.Application.InternalApi.Models;
using UniCast.Domain.Students.ValueObjects;
using UniCast.InternalApi.Dto;

namespace UniCast.InternalApi.Endpoints.V1.Messages;

public sealed class SendIndividualMessageEndpoint : Ep.Req<SendIndividualMessageRequest>.NoRes
{
    private readonly SendIndividualMessageCommandHandler _handler;

    public SendIndividualMessageEndpoint(SendIndividualMessageCommandHandler handler)
    {
        _handler = handler;
    }

    public override void Configure()
    {
        Post("/internal-api/v1/messages/send-individual");
        AllowAnonymous();
        Validator<SendIndividualMessageRequestValidator>();
    }

    public override async Task HandleAsync(SendIndividualMessageRequest req, CancellationToken ct)
    {
        var result = await _handler.HandleAsync(
            new SendIndividualMessageCommand(
                req.Students.Select(
                        x => new StudentModel(
                            StudentFullName.From(x.FullName),
                            AcademicGroupName.From(x.GroupName))
                    )
                    .ToList(),
                req.Message,
                req.SenderUsername,
                req.SenderId
            ),
            ct);

        if (result.IsSuccess)
        {
            await SendOkAsync(ct);
            return;
        }

        await SendAsync(new { Error = result.Error.Message }, statusCode: (int)result.Error.Group, ct);
    }
}

public readonly record struct SendIndividualMessageRequest(
    List<StudentDto> Students,
    string Message,
    string SenderUsername,
    int SenderId
);

public sealed class SendIndividualMessageRequestValidator : AbstractValidator<SendIndividualMessageRequest>
{
    public SendIndividualMessageRequestValidator()
    {
        var studentDtoValidator = new StudentDtoValidator();

        RuleForEach(x => x.Students)
            .SetValidator(studentDtoValidator);

        RuleFor(x => x.Message)
            .NotEmpty();

        RuleFor(x => x.SenderUsername)
            .NotEmpty();
    }
}