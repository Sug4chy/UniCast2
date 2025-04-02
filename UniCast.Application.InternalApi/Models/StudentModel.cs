using UniCast.Domain.Students.ValueObjects;

namespace UniCast.Application.InternalApi.Models;

public readonly record struct StudentModel(
    StudentFullName FullName,
    AcademicGroupName GroupName
);