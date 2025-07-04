using System.Net.Http.Json;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Options;
using UniCast.Application.Abstractions.Moodle;
using UniCast.Application.Result;
using UniCast.Domain.Students.Entities;
using UniCast.Infrastructure.Moodle.Configuration;
using UniCast.Infrastructure.Moodle.Errors;
using UniCast.Infrastructure.Moodle.Responses;

namespace UniCast.Infrastructure.Moodle.Client;

public sealed class MoodleApiClient : IMoodleClient
{
    private readonly HttpClient _httpClient;
    private readonly MoodleConfiguration _configuration;

    public MoodleApiClient(HttpClient httpClient, IOptions<MoodleConfiguration> configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration.Value;
    }

    public async Task<Result<string>> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        try
        {
            var responseMessage = await _httpClient.PostAsync(
                requestUri: $"{_configuration.BaseUrl}/login/token.php?username={username}&password={password}" +
                            $"&service=moodle_mobile_app",
                content: null,
                cancellationToken: ct);
            responseMessage.EnsureSuccessStatusCode();

            var response = await responseMessage.Content.ReadFromJsonAsync<Dictionary<string, string>>(ct);

            return response!.TryGetValue("token", out string? token)
                ? Result.Success(token)
                : Result.Failure<string>(response.GetValueOrDefault("error"));
        }
        catch (Exception ex)
        {
            return Result.Failure<string>(ex.Message);
        }
    }

    public async Task<Result<long, Error>> SendMessageAsync(
        string senderToken,
        int receiverExtId,
        string text,
        CancellationToken ct = default)
    {
        try
        {
            var responseMessage = await _httpClient.PostAsync(
                requestUri: $"{_configuration.BaseUrl}/webservice/rest/server.php?wstoken={senderToken}" +
                            $"&wsfunction=core_message_send_instant_messages" +
                            $"&moodlewsrestformat=json" +
                            $"&messages[0][touserid]={receiverExtId}" +
                            $"&messages[0][text]={text}" +
                            $"&messages[0][textformat]=0",
                content: null,
                cancellationToken: ct);
            responseMessage.EnsureSuccessStatusCode();

            string responseString = await responseMessage.Content.ReadAsStringAsync(ct);
            if (!(responseString.StartsWith('[') && responseString.EndsWith(']')))
            {
                return AccessError.TryParse(responseString, out var error)
                    ? Result.Failure<long, Error>(Error.Of(error.Message, ErrorGroup.AccessError))
                    : Result.Failure<long, Error>(Error.Of(responseString));
            }

            var deserializedResponse = JsonSerializer.Deserialize<SentMessageInfo[]>(responseString)!;

            return Result.Success<long, Error>(deserializedResponse[0].MessageId);
        }
        catch (Exception e)
        {
            return Result.Failure<long, Error>(Error.Of(e.Message));
        }
    }

    public async Task<UnitResult<Error>> OrderReferenceForStudentAsync(Student student, CancellationToken ct = default)
    {
        var result = await SendMessageAsync(
            senderToken: student.MoodleAccount!.CurrentToken!,
            receiverExtId: _configuration.IssuingMethodologistExtId,
            text: "Здравствуйте, хочу заказать справку о том, что являюсь студентом",
            ct: ct
        );

        return result.IsSuccess ? UnitResult.Success<Error>() : UnitResult.Failure(result.Error);
    }
}