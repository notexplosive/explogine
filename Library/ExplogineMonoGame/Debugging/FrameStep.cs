using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using ExTween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

public class FrameStep : IUpdateInput
{
    private readonly TweenableFloat _lineThickness = new();
    private readonly TweenableFloat _opacity = new();
    private readonly TweenableFloat _shrinkAmount = new();
    private readonly SequenceTween _tween = new();
    private bool _shouldDisplay;

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (Client.Debug.IsPassiveOrActive)
        {
            if (Client.CartridgeChain.IsFrozen)
            {
                if (input.Mouse.ScrollDelta(true) < 0 || input.Keyboard.GetButton(Keys.OemPipe, true).WasPressed)
                {
                    Step();
                }
            }

            if (input.Keyboard.GetButton(Keys.Space).WasPressed && input.Keyboard.Modifiers.Control)
            {
                Toggle();
            }
        }
    }

    private void Step()
    {
        Client.CartridgeChain.UpdateCurrentCartridge(1 / 60f);

        _tween.Clear();
        _shrinkAmount.Value = 0f;

        // Animate Step
        _tween
            .Add(new Tween<float>(_shrinkAmount, 10, 0.15f, Ease.CubicFastSlow))
            .Add(new Tween<float>(_shrinkAmount, 0, 0.15f, Ease.CubicSlowFast))
            ;
    }

    private void Toggle()
    {
        Client.CartridgeChain.IsFrozen = !Client.CartridgeChain.IsFrozen;

        if (Client.CartridgeChain.IsFrozen)
        {
            _tween.Clear();
            _lineThickness.Value = 0f;
            _shrinkAmount.Value = 0f;

            // Animate Freeze
            _tween
                .Add(new CallbackTween(() => _shouldDisplay = true))
                .Add(
                    new MultiplexTween()
                        .AddChannel(new Tween<float>(_lineThickness, 5, 0.15f, Ease.CubicSlowFast))
                        .AddChannel(
                            new SequenceTween()
                                .Add(new Tween<float>(_shrinkAmount, 10, 0.15f, Ease.CubicSlowFast))
                                .Add(new Tween<float>(_shrinkAmount, 0, 0.15f, Ease.CubicFastSlow))

                        )
                )
                ;
        }
        else
        {
            _tween.Clear();

            // Animate Unfreeze
            _tween
                .Add(new Tween<float>(_lineThickness, 0, 0.15f, Ease.CubicFastSlow))
                .Add(new CallbackTween(() => _shouldDisplay = false))
                ;
        }
    }

    public void Draw(Painter painter, Depth depth)
    {
        if (_shouldDisplay)
        {
            var rect = new RectangleF(Vector2.Zero, Client.Window.Size.ToVector2());
            var inset = _shrinkAmount + 5;
            painter.DrawLineRectangle(rect.Inflated(-inset, -inset),
                new LineDrawSettings
                    {Color = Color.Cyan.WithMultipliedOpacity(1f), Depth = depth, Thickness = _lineThickness});
        }
    }

    public void UpdateGraphic(float dt)
    {
        _tween.Update(dt);
    }
}
