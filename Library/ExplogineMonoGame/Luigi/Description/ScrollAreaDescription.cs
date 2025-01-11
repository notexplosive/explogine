using System;
using ExplogineCore.Data;
using ExplogineCore.Lua;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Luigi.Description;

[LuaBoundType]
public class ScrollAreaDescription : IGuiDescription
{
    private readonly LuaLayoutBuilderBlock _childGroup;
    private readonly string? _scrollPositionId;

    public ScrollAreaDescription(LuaLayoutBuilderBlock childGroup, string? scrollPositionId)
    {
        _childGroup = childGroup;
        _scrollPositionId = scrollPositionId;
    }

    public void Apply(Gui.Gui gui, RectangleF rectangle, LuaGuiBindingContext context, LuaLayoutBuilderBlock block,
        LuaLayoutBuilder luaLayoutBuilder)
    {
        var panelRectangle = rectangle;
        var panel = gui.Panel(panelRectangle, Depth.Middle, _childGroup.Theme);

        var internalRectangle = new RectangleF(Vector2.Zero, new Vector2(rectangle.Width, float.MaxValue));
        var baked = _childGroup.Bake(internalRectangle);

        var hoverState = new HoverState();
        panel.AddUpdateInputBehavior((input, hitTestStack) =>
        {
            hitTestStack.AddInfiniteZone(Depth.Front, hoverState, true);

            if (hoverState)
            {
                var delta = input.Mouse.ScrollDelta() / 2f;
                input.Mouse.ConsumeScrollDelta();
                panel.ScrollPositionY = Math.Clamp(panel.ScrollPositionY - delta, 0,
                    Math.Max(0, baked.UsedSpace.Bottom - panelRectangle.Height));

                if (_scrollPositionId != null)
                {
                    context.RememberedState.SetScrollPosition(_scrollPositionId, panel.ScrollPositionY);
                }
            }
        });

        luaLayoutBuilder.ApplyInstructions(_childGroup, baked, context, panel.InnerGui);

        if (_scrollPositionId != null)
        {
            context.OnFinalize += () =>
            {
                panel.ScrollPositionY = context.RememberedState.GetScrollPosition(_scrollPositionId);
            };
        }
    }
}
