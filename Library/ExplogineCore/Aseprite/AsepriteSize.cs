using Newtonsoft.Json;

namespace ExplogineCore.Aseprite;

[Serializable]
public struct AsepriteSize
{
    [JsonProperty("w")]
    public int Width { get; set; }

    [JsonProperty("h")]
    public int Height { get; set; }
}
