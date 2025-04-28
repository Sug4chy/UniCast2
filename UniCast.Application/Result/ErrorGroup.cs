namespace UniCast.Application.Result;

public enum ErrorGroup
{
    AccessError = 401,
    NotFound = 404,
    DomainError = 422,
    InternalError = 500,
}