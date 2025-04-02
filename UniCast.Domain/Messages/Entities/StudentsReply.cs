using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Students.Entities;

namespace UniCast.Domain.Messages.Entities;

public sealed class StudentsReply : Entity<IdOf<StudentsReply>>
{
    public required string ReplyText { get; init; }

    public IdOf<Student> StudentId { get; init; }
    public Student? Student { get; init; }

    public IdOf<MessageFromMethodist> MessageId { get; init; }
    public MessageFromMethodist? Message { get; init; }

    public static StudentsReply Create(
        IdOf<StudentsReply> id,
        string replyText,
        Student student,
        MessageFromMethodist message)
        => new()
        {
            Id = id,
            ReplyText = replyText,
            StudentId = student.Id,
            Student = student,
            MessageId = message.Id,
            Message = message,
        };
}