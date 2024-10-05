using Newtonsoft.Json;

namespace ExplogineCore.Aseprite;

[Serializable]
public class AsepriteFrameTag
{
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("from")]
    public int From { get; set; }

    [JsonProperty("to")]
    public int To { get; set; }

    [JsonProperty("direction")]
    public string Direction { get; set; } = string.Empty;

    [JsonProperty("color")]
    public string Color { get; set; } = string.Empty;

    [JsonProperty("data")]
    public string UserData { get; set; } = string.Empty;
}
