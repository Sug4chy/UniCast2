using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using UniCast.Application.Abstractions.Moodle;
using UniCast.Application.Result;
using UniCast.Infrastructure.Moodle.Errors;

namespace UniCast.Infrastructure.Moodle.Client;

public sealed class MoodleApiClient : IMoodleClient
{
    private readonly HttpClient _httpClient;
    private readonly string _moodleUrl;

    public MoodleApiClient(HttpClient httpClient, string moodleUrl)
    {
        _httpClient = httpClient;
        _moodleUrl = moodleUrl;
    }

    public async Task<Result<string>> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        try
        {
            var responseMessage = await _httpClient.PostAsync(
                requestUri: $"{_moodleUrl}/login/token.php?username={username}&password={password}" +
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

    public async Task<UnitResult<Error>> SendMessageAsync(
        string senderToken,
        int receiverExtId,
        string text,
        CancellationToken ct = default)
    {
        try
        {
            var responseMessage = await _httpClient.PostAsync(
                requestUri: $"{_moodleUrl}/webservice/rest/server.php?wstoken={senderToken}" +
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
                    ? UnitResult.Failure(Error.Of(error.Message, ErrorGroup.AccessError))
                    : UnitResult.Failure(Error.Of(responseString));
            }

            return UnitResult.Success<Error>();
        }
        catch (Exception e)
        {
            return UnitResult.Failure(Error.Of(e.Message));
        }
    }
}