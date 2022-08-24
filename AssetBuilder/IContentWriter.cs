namespace AssetBuilder;

internal interface IContentWriter
{
    IEnumerable<string> GetContentLines(string relativePath);
}

public class TextureContentWriter : IContentWriter
{
    public IEnumerable<string> GetContentLines(string relativePath)
    {
        yield return $"#begin {relativePath}";
        yield return "/importer:TextureImporter";
        yield return "/processor:TextureProcessor";
        yield return "/processorParam:ColorKeyColor=255,0,255,255";
        yield return "/processorParam:ColorKeyEnabled=True";
        yield return "/processorParam:GenerateMipmaps=False";
        yield return "/processorParam:PremultiplyAlpha=True";
        yield return "/processorParam:ResizeToPowerOfTwo=False";
        yield return "/processorParam:MakeSquare=False";
        yield return "/processorParam:TextureFormat=Color";
        yield return $"/build:{relativePath}";
    }
}

public class OggContentWriter : IContentWriter
{
    public IEnumerable<string> GetContentLines(string relativePath)
    {
        yield return $"#begin {relativePath}";
        yield return "/importer:OggImporter";
        yield return "/processor:SoundEffectProcessor";
        yield return "/processorParam:Quality=Best";
        yield return $"/build:{relativePath}";
    }
}

public class CopyContentWriter : IContentWriter
{
    public IEnumerable<string> GetContentLines(string relativePath)
    {
        yield return $"#begin {relativePath}";
        yield return $"/copy:{relativePath}";
    }
}

public class SpriteFontContentWriter : IContentWriter
{
    public IEnumerable<string> GetContentLines(string relativePath)
    {
        yield return $"#begin {relativePath}";
        yield return "/importer:FontDescriptionImporter";
        yield return "/processor:FontDescriptionProcessor";
        yield return "/processorParam:PremultiplyAlpha=True";
        yield return "/processorParam:TextureFormat=Compressed";
        yield return $"/build:{relativePath}";
    }
}
