using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Students.Entities;

namespace UniCast.Infrastructure.Persistence.Entities;

public sealed record MessageFromMethodistStudent
{
    public required IdOf<Student> StudentId { get; init; }
    public required IdOf<MessageFromMethodist> MessageId { get; init; }
}