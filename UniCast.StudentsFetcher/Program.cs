#pragma warning disable IL3050
#pragma warning disable IL2026
// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Microsoft.EntityFrameworkCore;
using UniCast.Domain.Common.ValueObjects;
using UniCast.Domain.Moodle;
using UniCast.Domain.Students.Entities;
using UniCast.Domain.Students.ValueObjects;
using UniCast.Infrastructure.Persistence.Context;
using UniCast.Infrastructure.Persistence.Context.Options;

string moodleBaseUrl = GetEnvironmentVariable("MOODLE_BASE_URL");
string token = GetEnvironmentVariable("MOODLE_TOKEN");
string conString = GetEnvironmentVariable("CONNECTION_STRING");

Console.WriteLine("Read env variables...");

using var httpClient = new HttpClient();
await using var dbContext = new PostgresqlDataContext(DbContextOptionsFactory.Build<PostgresqlDataContext>(conString));

var usersResponse = await httpClient.GetFromJsonAsync<CoreUserGetUsersResponse>(
    $"{moodleBaseUrl}/webservice/rest/server.php" +
    $"?wstoken={token}&wsfunction=core_user_get_users&moodlewsrestformat=json" +
    "&criteria[0][key]=suspended&criteria[0][value]=false",
    new JsonSerializerOptions { TypeInfoResolver = new DefaultJsonTypeInfoResolver() });

Console.WriteLine("Users fetched...");

var group = await dbContext.AcademicGroups.FirstAsync(x => x.Name == AcademicGroupName.From("ПрИ-404"));
foreach (var user in usersResponse!.Users)
{
    string[] studentFullNameParts = user.FullName.Split(' ',
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    if (studentFullNameParts.Length != 2)
    {
        Console.WriteLine($"User with strange name. ID: {user.Id}. Full name: {user.FullName}");
        continue;
    }

    var student = Student.Create(
        id: IdOf<Student>.New(),
        fullName: StudentFullName.From($"{studentFullNameParts[1]} {studentFullNameParts[0]}"),
        group: group
    );
    var moodleAccount = new MoodleAccount(IdOf<MoodleAccount>.New())
    {
        ExtId = user.Id,
        Username = user.Username,
        StudentId = student.Id,
        Student = student
    };

    dbContext.Students.Add(student);
    dbContext.MoodleAccounts.Add(moodleAccount);
    await dbContext.SaveChangesAsync();
}

Console.WriteLine("Users loaded.");

return;

string GetEnvironmentVariable(string name)
{
    string? env = Environment.GetEnvironmentVariable(name);
    if (env is null)
    {
        throw new ArgumentException($"Missing environment variable {name}");
    }

    return env;
}

internal sealed class CoreUserGetUsersResponse
{
    [JsonPropertyName("users")] public required CoreUserDto[] Users { get; init; }
}

internal sealed class CoreUserDto
{
    [JsonPropertyName("id")] public int Id { get; init; }
    [JsonPropertyName("fullname")] public required string FullName { get; init; }
    [JsonPropertyName("username")] public required string Username { get; init; }
}