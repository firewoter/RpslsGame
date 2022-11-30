using System.Text.Json.Serialization;

namespace Game.Infrastructure;

public class ResponseModel
{
    [JsonPropertyName("random_number")]
    public int RandomNumber { get; set; }
}