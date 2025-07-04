using System.Text.Json.Serialization;

namespace UniCast.Infrastructure.Moodle.Responses;

public readonly record struct SentMessageInfo(
    [property: JsonPropertyName("msgid")] long MessageId
);