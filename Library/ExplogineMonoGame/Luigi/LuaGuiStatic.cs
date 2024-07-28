using ExplogineCore.Data;
using ExplogineCore.Lua;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Layout;
using ExplogineMonoGame.Luigi.Description;
using JetBrains.Annotations;

namespace ExplogineMonoGame.Luigi;

/// <summary>
/// Supplies the static lua objects
/// </summary>
public static class LuaGuiStatic
{
    public static FillFactory Fill { get; } = new();
    public static GuiFactory Gui { get; } = new();
    public static AlignmentFactory Alignment { get; } = new();
    public static OrientationFactory Orientation { get; } = new();

    /// <summary>
    /// This is the `fill` object
    /// </summary>
    public class FillFactory
    {
        internal FillFactory()
        {
        }

        [UsedImplicitly]
        [LuaMember("horizontal")]
        public LayoutElement Horizontal(float verticalSize)
        {
            return L.FillHorizontal(verticalSize);
        }

        [UsedImplicitly]
        [LuaMember("vertical")]
        public LayoutElement Vertical(float horizontalSize)
        {
            return L.FillVertical(horizontalSize);
        }

        [UsedImplicitly]
        [LuaMember("both")]
        public LayoutElement Both()
        {
            return L.FillBoth();
        }
        
        [UsedImplicitly]
        [LuaMember("fixed")]
        public LayoutElement Fixed(float width, float? height = null)
        {
            return L.FixedElement(width, height ?? width);
        }
    }

    /// <summary>
    /// This is the `gui` object
    /// </summary>
    public class GuiFactory
    {
        internal GuiFactory()
        {
        }

        [UsedImplicitly]
        [LuaMember("label")]
        public IGuiDescription Label(string text, int? fontSize = null)
        {
            return new LabelGuiDescription(text, fontSize);
        }

        [UsedImplicitly]
        [LuaMember("graphic")]
        public IGuiDescription Graphic(LuaGuiCommand commandName)
        {
            return new GraphicGuiDescription(commandName);
        }
        
        [UsedImplicitly]
        [LuaMember("button")]
        public IGuiDescription Button(LuaGuiCommand command, string? label = null)
        {
            return new ButtonGuiDescription(command, label ?? "");
        }
        
        [UsedImplicitly]
        [LuaMember("textField")]
        public IGuiDescription TextField(LuaGuiCommand submitCommand, LuaGuiCommand? createCommand)
        {
            return new TextFieldGuiDescription(submitCommand, createCommand);
        }
        
        [UsedImplicitly]
        [LuaMember("tag")]
        public IGuiDescription Tag(string tagName)
        {
            return new TagElementGuiDescription(tagName);
        }
    }

    public class AlignmentFactory
    {
        internal AlignmentFactory()
        {
        }

        [UsedImplicitly]
        [LuaMember("topLeft")]
        public Alignment TopLeft()
        {
            return ExplogineMonoGame.Data.Alignment.TopLeft;
        }

        [UsedImplicitly]
        [LuaMember("topCenter")]
        public Alignment TopCenter()
        {
            return ExplogineMonoGame.Data.Alignment.TopCenter;
        }

        [UsedImplicitly]
        [LuaMember("topRight")]
        public Alignment TopRight()
        {
            return ExplogineMonoGame.Data.Alignment.TopRight;
        }

        [UsedImplicitly]
        [LuaMember("centerLeft")]
        public Alignment CenterLeft()
        {
            return ExplogineMonoGame.Data.Alignment.CenterLeft;
        }

        [UsedImplicitly]
        [LuaMember("center")]
        public Alignment CenterCenter()
        {
            return ExplogineMonoGame.Data.Alignment.Center;
        }

        [UsedImplicitly]
        [LuaMember("CenterRight")]
        public Alignment CenterRight()
        {
            return ExplogineMonoGame.Data.Alignment.CenterRight;
        }

        [UsedImplicitly]
        [LuaMember("BottomLeft")]
        public Alignment BottomLeft()
        {
            return ExplogineMonoGame.Data.Alignment.BottomLeft;
        }

        [UsedImplicitly]
        [LuaMember("BottomCenter")]
        public Alignment BottomCenter()
        {
            return ExplogineMonoGame.Data.Alignment.BottomCenter;
        }

        [UsedImplicitly]
        [LuaMember("BottomRight")]
        public Alignment BottomRight()
        {
            return ExplogineMonoGame.Data.Alignment.BottomRight;
        }
    }
    
    public class OrientationFactory
    {
        internal OrientationFactory()
        {
        }

        [UsedImplicitly]
        [LuaMember("vertical")]
        public Orientation Vertical()
        {
            return ExplogineCore.Data.Orientation.Vertical;
        }

        [UsedImplicitly]
        [LuaMember("horizontal")]
        public Orientation Horizontal()
        {
            return ExplogineCore.Data.Orientation.Horizontal;
        }
    }
}
