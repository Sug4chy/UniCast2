using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Domain.Students.ValueObjects.Enums;

namespace UniCast.Domain.Students.Entities;

/// <summary>
/// Студент, который учится на ИИТ
/// </summary>
public sealed class Student : Entity<IdOf<Student>>
{
    private readonly List<MessageFromMethodist> _messages;

    /// <summary>
    /// ФИО студента
    /// </summary>
    public StudentFullName FullName { get; private set; }

    /// <summary>
    /// Академическая группа, к которой принадлежит студент
    /// </summary>
    public AcademicGroup Group { get; private set; }

    /// <summary>
    /// Флаги, которые указывают, какие способы связи есть с этим студентом
    /// </summary>
    public CommunicationMethod CommunicationMethods { get; private set; }

    public IReadOnlyList<MessageFromMethodist> Messages => _messages.AsReadOnly();

    private Student(
        IdOf<Student> id,
        StudentFullName fullName,
        AcademicGroup group,
        CommunicationMethod communicationMethods, 
        Maybe<List<MessageFromMethodist>> messages) : base(id)
    {
        FullName = fullName;
        Group = group;
        CommunicationMethods = communicationMethods;
        _messages = messages.GetValueOrDefault([]);
    }

    public static Result<Student> Create(
        StudentFullName fullName,
        AcademicGroup group,
        List<MessageFromMethodist> messages,
        CommunicationMethod communicationMethods = CommunicationMethod.None)
    {
        if (group is null)
        {
            return Result.Failure<Student>("Отсутствует группа");
        }

        return Result.Success(new Student(
            id: IdOf<Student>.New(),
            fullName: fullName,
            group: group,
            communicationMethods: communicationMethods,
            messages: messages));
    }
}