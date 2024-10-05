using Newtonsoft.Json;

namespace ExplogineCore.Aseprite;

[Serializable]
public class AsepriteMetaData
{
    [JsonProperty("frameTags")]
    public List<AsepriteFrameTag> FrameTags { get; set; } = new();
}
