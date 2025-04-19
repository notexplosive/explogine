using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Layout;

public static class LayoutSerialization
{
    private static string[] SerializeSize(IEdgeSize x, IEdgeSize y)
    {
        return new[]
        {
            x.Serialized(),
            y.Serialized()
        };
    }

    private static LayoutElement[] DeserializeElements(SerializedElement[] elements)
    {
        var result = new LayoutElement[elements.Length];

        for (var i = 0; i < elements.Length; i++)
        {
            var element = elements[i];
            result[i] = element.Deserialize();
        }

        return result;
    }

    public struct SerializedSettings
    {
        public int Orientation;
        public float[] Margin;
        public string Alignment;
        public int Padding;

        public SerializedSettings(Style settings)
        {
            Alignment = settings.Alignment.ToString();
            Margin = new[] {settings.Margin.X, settings.Margin.Y};
            Orientation = (int) settings.Orientation;
            Padding = settings.PaddingBetweenElements;
        }

        public Style Deserialize()
        {
            return new Style((Orientation) Orientation, Padding, new Vector2(Margin[0], Margin[1]),
                DeserializeAlignment(Alignment));
        }

        private Alignment DeserializeAlignment(string serializedAlignment)
        {
            // Alignment's ToString should be "{Horizontal} {Vertical}"
            var split = serializedAlignment.Split(' ');
            if (Enum.TryParse(split[0], out HorizontalAlignment horizontal) &&
                Enum.TryParse(split[1], out VerticalAlignment vertical))
            {
                return new Alignment(horizontal, vertical);
            }

            throw new Exception($"Could not deserialize Alignment: {serializedAlignment}");
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

        public LayoutElement Deserialize()
        {
            return new LayoutElement(
                Name != null ? new ElementName(Name) : new ElementBlankName(),
                DeserializeEdgeSize(Size[0]),
                DeserializeEdgeSize(Size[1]),
                SubGroup?.Deserialize()
            );
        }

        private IEdgeSize DeserializeEdgeSize(string edgeSize)
        {
            return edgeSize == "fill" ? new FillEdgeSize() : new FixedEdgeSize(int.Parse(edgeSize));
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
            Settings = new SerializedSettings(layoutLayoutElements.Style);
        }

        public LayoutElementGroup Deserialize()
        {
            return new LayoutElementGroup(Settings.Deserialize(), DeserializeElements(Elements));
        }
    }
}
