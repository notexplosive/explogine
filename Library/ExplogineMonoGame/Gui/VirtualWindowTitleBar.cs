using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Layout;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Gui;

partial class VirtualWindow
{
    public class TitleBar : IUpdateInputHook, IDrawHook
    {
        private readonly Chrome _chrome;
        private readonly VirtualWindow _parentWindow;
        private LayoutArrangement _layout = null!;
        private Depth Depth => _parentWindow.StartingDepth - 1;

        public TitleBar(VirtualWindow parentWindow, Chrome chrome)
        {
            _parentWindow = parentWindow;
            _chrome = chrome;
            OnResized();

            _chrome.Resized += OnResized;
        }

        public void Draw(Painter painter)
        {
            // Todo: this should go away, the theme should draw all this stuff
            var currentLayout = GetLayoutStruct();
            
            painter.DrawRectangle(currentLayout.Icon, new DrawSettings{Color = Color.White, Depth = Depth});

            foreach (var button in currentLayout.ButtonsRightToLeft)
            {
                painter.DrawRectangle(button, new DrawSettings {Color = Color.Red, Depth = Depth});
            }
        }

        public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
        {
            var currentLayout = GetLayoutStruct();
        }

        public Layout GetLayoutStruct()
        {
            var icon = _layout.FindElement("icon").Rectangle.Moved(_parentWindow.Position);
            var titleArea = _layout.FindElement("title").Rectangle.Moved(_parentWindow.Position);

            var buttonElements = _layout.FindElements("control-button");
            buttonElements.Reverse();
            var buttonRectsRightToLeft = new RectangleF[buttonElements.Count];
            var i = 0;
            foreach (var buttonElement in buttonElements)
            {
                buttonRectsRightToLeft[i++] = buttonElement.Rectangle.Moved(_parentWindow.Position);
            }

            return new Layout(icon, titleArea, buttonRectsRightToLeft);
        }

        private void OnResized()
        {
            GenerateLayout();
        }

        private void GenerateLayout()
        {
            var titleBarThickness = _chrome.TitleBarRectangle.Height;
            var margin = 2;
            var usableSize = titleBarThickness - margin * 2;
            _layout = L.Compute(_chrome.TitleBarRectangle.WithPosition(Vector2.Zero), L.Root(
                new Style(Orientation.Horizontal, 2, new Vector2(0, margin), Alignment.TopLeft),
                L.FixedElement("icon", usableSize, usableSize),
                L.FillVertical(5),
                L.FillHorizontal("title", usableSize),
                L.FixedElement("control-button", usableSize, usableSize),
                L.FixedElement("control-button", usableSize, usableSize),
                L.FixedElement("control-button", usableSize, usableSize)
            ));
        }

        public readonly record struct Layout(RectangleF Icon, RectangleF TitleArea,
            RectangleF[] ButtonsRightToLeft);
    }
}
