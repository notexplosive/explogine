using Newtonsoft.Json;

namespace ExplogineMonoGame.Layout;

public static class LayoutSerialization
{
    public static string ToJson(LayoutArrangement arrangement)
    {
        return JsonConvert.SerializeObject(new SerializedGroup(arrangement.RawGroup), Formatting.Indented);
    }

    private static string[] SerializeSize(IEdgeSize x, IEdgeSize y)
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
        
        public SerializedSettings(ArrangementSettings settings)
        {
            Alignment = settings.Alignment.ToString();
            Margin = new[] {settings.Margin.X, settings.Margin.Y};
            Orientation = (int) settings.Orientation;
        }
    }

    public struct SerializedElement
    {
        public string? Name;
        public string[] Size;
        public SerializedGroup? SubGroup;
        
        public SerializedElement(LayoutElement layoutElement)
        {
            SubGroup = layoutElement.Children.HasValue
                ? new SerializedGroup(layoutElement.Children.Value)
                : null;
            Size = SerializeSize(layoutElement.X, layoutElement.Y);
            Name = layoutElement.Name is ElementName name ? name.Text : null;
        }
    }

    public struct SerializedGroup
    {
        public SerializedSettings Settings;
        public SerializedElement[] Elements;
        
        public SerializedGroup(LayoutElementGroup layoutLayoutElements)
        {
            var elements = new SerializedElement[layoutLayoutElements.Elements.Length];

            for (var i = 0; i < layoutLayoutElements.Elements.Length; i++)
            {
                elements[i] = new SerializedElement(layoutLayoutElements.Elements[i]);
            }

            Elements = elements;
            Settings = new SerializedSettings(layoutLayoutElements.ArrangementSettings);
        }
    }
}
