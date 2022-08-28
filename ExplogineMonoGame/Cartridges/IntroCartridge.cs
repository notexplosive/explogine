using System;
using ExplogineMonoGame.Data;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ExplogineMonoGame.Cartridges;

public class IntroCartridge : ICartridge
{
    private Figure _figure;
    private Font? _logoFont;
    private SequenceTween _tween = new();

    public void OnCartridgeStarted()
    {
        Client.Debug.Log("Intro loaded");
        _logoFont = Client.Assets.GetFont("engine/logo-font", 72);
        _figure = new Figure();
        _tween = Client.Random.Dirty.GetRandomElement(new Func<SequenceTween>[] {Tada})();
        _tween.Add(new WaitSecondsTween(0.5f));
    }

    public void Update(float dt)
    {
        _tween.Update(dt);
    }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(SamplerState.LinearClamp,
            Matrix.CreateTranslation(new Vector3(-Client.Window.RenderResolution.ToVector2() / 2, 0))
            * Matrix.CreateScale(new Vector3(new Vector2(_figure.Scale), 1))
            * Matrix.CreateTranslation(new Vector3(Client.Window.RenderResolution.ToVector2() / 2, 0))
        );
        painter.Clear(Color.Navy);

        var centerOfScreen = (Client.Window.RenderResolution.ToVector2() / 2).ToPoint();

        var text = "NotExplosive.net";
        painter.DrawStringAtPosition(_logoFont!, text, centerOfScreen + _figure.Position.Value.ToPoint(),
            new DrawSettings {Origin = DrawOrigin.Center, Angle = _figure.Angle});

        painter.EndSpriteBatch();
    }

    public bool ShouldLoadNextCartridge()
    {
        return _tween.IsDone();
    }

    private SequenceTween Tada()
    {
        var increment = 0.05f;

        return
            new SequenceTween()
                .Add(new Tween<float>(_figure.Scale, 1.25f, 0.5f, Ease.QuadFastSlow))
                .Add(new MultiplexTween()
                    .AddChannel(new DynamicTween(() =>
                        {
                            var sequence = new SequenceTween();
                            var flip = 0;
                            var shakeDistance = 15f;
                            var angleRange = 0.05f;
                            for (var i = 0f; i < 0.5f; i += increment)
                            {
                                flip++;
                                var even = flip % 2 == 0;
                                sequence.Add(new MultiplexTween()
                                    .AddChannel(new Tween<float>(_figure.Angle, even ? angleRange : -angleRange,
                                        increment,
                                        Ease.Linear))
                                );
                            }

                            return sequence;
                        })
                    )
                )
                .Add(new MultiplexTween()
                    .AddChannel(new Tween<Vector2>(_figure.Position,
                        new Vector2(0, 0), increment, Ease.Linear))
                    .AddChannel(new Tween<float>(_figure.Angle, 0, increment,
                        Ease.Linear))
                )
                .Add(new WaitSecondsTween(0.05f))
                .Add(new Tween<float>(_figure.Scale, 1f, 0.5f, Ease.QuadFastSlow))
            ;
    }

    private readonly struct Figure
    {
        public TweenableFloat Angle { get; } = new();
        public TweenableVector2 Position { get; } = new();
        public TweenableFloat Scale { get; } = new(1);

        public Figure()
        {
        }
    }
}
