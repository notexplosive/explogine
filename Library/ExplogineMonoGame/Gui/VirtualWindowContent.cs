using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Gui;

partial class VirtualWindow
{
    public class Content : IUpdateInputHook
    {
        private readonly Clickable _bodyClickable = new();
        private readonly HoverState _contentHovered = new();
        private readonly VirtualWindow _parentWindow;

        public Content(VirtualWindow parentWindow)
        {
            _parentWindow = parentWindow;
            _bodyClickable.ClickInitiated += parentWindow.RequestFocus;
        }

        public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
        {
            var contentHitTestLayer = hitTestStack.AddLayer(
                _parentWindow.CanvasRectangle.ScreenToCanvas(_parentWindow.CanvasRectangle.Size.ToPoint()),
                _parentWindow.StartingDepth, _parentWindow.CanvasRectangle);
            contentHitTestLayer.AddInfiniteZone(Depth.Front, _contentHovered, true);
            contentHitTestLayer.AddInfiniteZone(Depth.Back, () =>
            {
                /*This is only here so we prevent penetrating hit tests*/
            });
            _bodyClickable.Poll(input.Mouse, _contentHovered);
        }
    }
}
