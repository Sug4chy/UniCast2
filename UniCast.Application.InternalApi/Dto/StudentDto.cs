using UniCast.Domain.Students.ValueObjects;

namespace UniCast.Application.InternalApi.Dto;

public readonly record struct StudentDto(
    StudentFullName FullName,
    AcademicGroupName GroupName
);