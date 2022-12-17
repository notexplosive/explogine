using Newtonsoft.Json;

namespace ExplogineMonoGame.Layout;

public static class LayoutSerialization
{
    public static string ToJson(LayoutArrangement arrangement)
    {
        return JsonConvert.SerializeObject(LayoutSerialization.SerializeGroup(arrangement.RawGroup), Formatting.Indented);
    }

    public static SerializedSettings SerializeSettings(ArrangementSettings settings)
    {
        var result = new SerializedSettings
        {
            Alignment = settings.Alignment.ToString(),
            Margin = new[] {settings.Margin.X, settings.Margin.Y},
            Orientation = (int) settings.Orientation
        };
        return result;
    }

    public static SerializedGroup SerializeGroup(LayoutElementGroup layoutLayoutElements)
    {
        var elements = new SerializedElement[layoutLayoutElements.Elements.Length];

        for (var i = 0; i < layoutLayoutElements.Elements.Length; i++)
        {
            elements[i] = LayoutSerialization.SerializeElement(layoutLayoutElements.Elements[i]);
        }

        return new SerializedGroup
        {
            Elements = elements,
            Settings = LayoutSerialization.SerializeSettings(layoutLayoutElements.ArrangementSettings)
        };
    }

    public static SerializedElement SerializeElement(LayoutElement layoutElement)
    {
        return new SerializedElement
        {
            SubGroup = layoutElement.Children.HasValue
                ? LayoutSerialization.SerializeGroup(layoutElement.Children.Value)
                : null,
            Size = LayoutSerialization.SerializeSize(layoutElement.X, layoutElement.Y),
            Name = layoutElement.Name is ElementName name ? name.Text : null
        };
    }

    public static string[] SerializeSize(IEdgeSize x, IEdgeSize y)
    {
        return new[]
        {
            x.Serialized(),
            y.Serialized()
        };
    }

    public struct SerializedSettings
    {
        public int Orientation;
        public float[] Margin;
        public string Alignment;
    }

    public struct SerializedElement
    {
        public string? Name;
        public string[] Size;
        public SerializedGroup? SubGroup;
    }

    public struct SerializedGroup
    {
        public SerializedSettings Settings;
        public SerializedElement[] Elements;
    }
}
