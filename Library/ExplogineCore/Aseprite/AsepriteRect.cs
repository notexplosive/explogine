using Newtonsoft.Json;

namespace ExplogineCore.Aseprite;

[Serializable]
public struct AsepriteRect
{
    [JsonProperty("x")]
    public int X { get; set; }
    
    [JsonProperty("y")]
    public int Y { get; set; }
    
    [JsonProperty("w")]
    public int Width { get; set; }
    
    [JsonProperty("h")]
    public int Height { get; set; }
}
