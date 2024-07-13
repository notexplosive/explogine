using Newtonsoft.Json;

namespace ExplogineCore.Aseprite;

[Serializable]
public class AsepriteSheetData
{
    [JsonProperty("frames")]
    public Dictionary<string, AsepriteFrame> Frames { get; set; } = new();
}