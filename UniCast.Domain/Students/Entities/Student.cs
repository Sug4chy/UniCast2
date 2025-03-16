using CSharpFunctionalExtensions;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Messages.Entities;
using UniCast.Domain.Students.ValueObjects;

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
    /// Сообщения, адресованные студенту
    /// </summary>
    public IReadOnlyList<MessageFromMethodist> Messages => _messages.AsReadOnly();

    private Student(
        IdOf<Student> id,
        StudentFullName fullName,
        AcademicGroup group,
        Maybe<List<MessageFromMethodist>> messages) : base(id)
    {
        FullName = fullName;
        Group = group;
        _messages = messages.GetValueOrDefault([]);
    }

    public static Student Create(
        IdOf<Student> id,
        StudentFullName fullName, 
        AcademicGroup group,
        Maybe<List<MessageFromMethodist>> messages = default) 
        => new(id, fullName, group, messages);
}