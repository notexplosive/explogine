using Newtonsoft.Json;

namespace ExplogineCore.Aseprite;

[Serializable]
public struct AsepriteFrame
{
    [JsonProperty("frame")]
    public AsepriteRect Frame { get; set; }
    
    [JsonProperty("rotated")]
    public bool Rotated { get; set; }
    
    [JsonProperty("trimmed")]
    public bool Trimmed { get; set; }
    
    [JsonProperty("duration")]
    public int Duration { get; set; }

    [JsonProperty("sourceSize")]
    public AsepriteSize SourceSize { get; set; }
    
    [JsonProperty("spriteSourceSize")]
    public AsepriteRect SpriteSourceRect { get; set; }
}
