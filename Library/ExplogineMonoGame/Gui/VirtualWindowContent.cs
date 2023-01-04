using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;

namespace ExplogineMonoGame.Gui;

partial class VirtualWindow
{
    public class Content : IUpdateInputHook
    {
        private readonly VirtualWindow _parentWindow;
        private readonly Clickable _bodyClickable = new();
        private readonly HoverState _contentHovered = new();
        
        public Content(VirtualWindow parentWindow)
        {
            _parentWindow = parentWindow;
            _bodyClickable.ClickInitiated += parentWindow.RequestFocus;
        }

        public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
        {
            var contentHitTestLayer = hitTestStack.AddLayer(_parentWindow.CanvasRectangle.ScreenToCanvas(_parentWindow.CanvasRectangle.Size.ToPoint()),
                _parentWindow.StartingDepth, _parentWindow.CanvasRectangle);
            contentHitTestLayer.AddInfiniteZone(Depth.Back, _contentHovered);
            _bodyClickable.Poll(input.Mouse, _contentHovered);
        }
    }
}