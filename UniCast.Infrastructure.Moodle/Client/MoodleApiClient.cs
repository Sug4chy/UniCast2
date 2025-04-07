using System.Net.Http.Json;
using CSharpFunctionalExtensions;
using UniCast.Application.Abstractions.Moodle;

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
}