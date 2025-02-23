using FastEndpoints;
using FluentValidation;

namespace UniCast.InternalApi.Endpoints.V1.Messages;

public sealed class SendIndividualMessageEndpoint : Ep.Req<SendIndividualMessageRequest>.NoRes
{
    public override void Configure()
    {
        Post("/internal-api/v1/messages/send-individual");
        Version(1);
        AllowAnonymous();
    }

    public override Task HandleAsync(SendIndividualMessageRequest req, CancellationToken ct)
    {
        return base.HandleAsync(req, ct);
    }
}

public readonly record struct SendIndividualMessageRequest(
);

public sealed class SendIndividualMessageRequestValidator : AbstractValidator<SendIndividualMessageRequest>
{
    public SendIndividualMessageRequestValidator()
    {
    }
}