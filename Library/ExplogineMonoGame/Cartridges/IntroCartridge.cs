using System;
using System.Collections.Generic;
using ExplogineMonoGame.Data;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Cartridges;

public class IntroCartridge : ICartridge
{
    private readonly uint _index;
    private readonly List<Figure> _letters = new();
    private readonly IndirectFont _logoFont = new("engine/logo-font", 72);
    private readonly string _text;
    private bool _cancelEarly;
    private bool _useWholeWord = true;
    private Figure _wholeWord;

    public IntroCartridge(string text, uint index)
    {
        _text = text;
        _index = index;
    }

    public SequenceTween Tween { get; private set; } = new();

    public void OnCartridgeStarted()
    {
        _wholeWord = new Figure(_text);

        foreach (var character in _text)
        {
            _letters.Add(new Figure(character.ToString()));
        }

        var tweens = new Func<SequenceTween>[]
        {
            ZoomFade,
            Tada,
            // Ouch,
            ZoomAndRotate,
            FlyInLetters,
            FlyInLettersRandom
        };

        Tween = tweens[_index % tweens.Length]();
        Tween.Add(new WaitSecondsTween(0.75f));
    }

    public void Update(float dt)
    {
        try
        {
            Tween.Update(dt);
        }
        catch (Exception e)
        {
            // If we somehow throw an exception during the intro, move onto the next cart
            Client.Debug.LogError($"Crashed during the intro {e}");
            _cancelEarly = true;
        }
    }

    public void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(
            Matrix.CreateTranslation(new Vector3(-Client.Window.RenderResolution.ToVector2() / 2, 0))
            * Matrix.CreateScale(new Vector3(new Vector2(_wholeWord.Scale), 1))
            * Matrix.CreateTranslation(new Vector3(Client.Window.RenderResolution.ToVector2() / 2, 0))
        );
        painter.Clear(Color.Navy);

        var centerOfScreen = Client.Window.RenderResolution.ToVector2() / 2;

        if (_useWholeWord)
        {
            painter.DrawStringAtPosition(_logoFont, _wholeWord.Text,
                centerOfScreen + _wholeWord.Position.Value,
                new DrawSettings
                {
                    Origin = DrawOrigin.Center, Angle = _wholeWord.Angle,
                    Color = Color.White.WithMultipliedOpacity(_wholeWord.Opacity)
                });
        }
        else
        {
            var runningWidth = 0f;
            foreach (var letter in _letters)
            {
                var myWidth = _logoFont.MeasureString(letter.Text).X;
                var textWidth = _logoFont.MeasureString(_wholeWord.Text).X;
                var startOffset = centerOfScreen - new Vector2(textWidth / 2f, 0) +
                                  new Vector2(runningWidth + myWidth / 2, 0);
                painter.DrawScaledStringAtPosition(_logoFont,
                    letter.Text,
                    startOffset + letter.Position.Value,
                    new Scale2D(letter.Scale),
                    new DrawSettings
                    {
                        Origin = DrawOrigin.Center,
                        Angle = letter.Angle,
                        Color = Color.White.WithMultipliedOpacity(letter.Opacity)
                    });
                runningWidth += myWidth;
            }
        }

        painter.EndSpriteBatch();
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (input.Keyboard.IsAnyKeyDown() || input.Mouse.WasAnyButtonPressedOrReleased())
        {
            Tween.SkipToEnd();
        }
    }

    public bool ShouldLoadNextCartridge()
    {
        return Tween.IsDone() || _cancelEarly;
    }

    public void Unload()
    {
    }

    public CartridgeConfig CartridgeConfig { get; } = new();

    private SequenceTween FlyInLetters()
    {
        _useWholeWord = false;

        var durationPerLetter = 0.5f;

        var result = new SequenceTween();
        result.Add(new CallbackTween(() =>
        {
            foreach (var letter in _letters)
            {
                letter.Scale.Value = 2f;
                letter.Opacity.Value = 0f;
            }
        }));
        result.Add(new DynamicTween(() =>
            {
                var index = 0;
                var multi = new MultiplexTween();
                foreach (var letter in _letters)
                {
                    multi.AddChannel(new SequenceTween()
                        .Add(new WaitSecondsTween(index * 0.05f))
                        .Add(new MultiplexTween()
                            .AddChannel(
                                new SequenceTween()
                                    .Add(new Tween<float>(letter.Scale, 0.95f, durationPerLetter * 3 / 4,
                                        Ease.QuadFastSlow))
                                    .Add(new Tween<float>(letter.Scale, 1f, durationPerLetter * 1 / 4,
                                        Ease.QuadSlowFast))
                            )
                            .AddChannel(new Tween<float>(letter.Opacity, 1f, durationPerLetter, Ease.Linear)))
                    );
                    index++;
                }

                return multi;
            }))
            .Add(new WaitSecondsTween(0.25f))
            ;

        return result;
    }

    private SequenceTween FlyInLettersRandom()
    {
        var durationPerLetter = 0.5f;
        _useWholeWord = false;

        var result = new SequenceTween();
        result.Add(new CallbackTween(() =>
        {
            foreach (var letter in _letters)
            {
                letter.Scale.Value = 4f;
                letter.Opacity.Value = 0f;
            }
        }));

        result.Add(new DynamicTween(() =>
            {
                var multi = new MultiplexTween();
                foreach (var letter in _letters)
                {
                    multi.AddChannel(new SequenceTween()
                        .Add(new WaitSecondsTween(Client.Random.Dirty.NextFloat()))
                        .Add(new MultiplexTween()
                            .AddChannel(
                                new SequenceTween()
                                    .Add(new Tween<float>(letter.Scale, 0.95f, durationPerLetter * 3 / 4,
                                        Ease.QuadFastSlow))
                                    .Add(new Tween<float>(letter.Scale, 1f, durationPerLetter * 1 / 4,
                                        Ease.QuadSlowFast))
                            )
                            .AddChannel(new Tween<float>(letter.Opacity, 1f, durationPerLetter, Ease.Linear)))
                    );
                }

                return multi;
            }))
            .Add(new WaitSecondsTween(0.25f))
            ;

        return result;
    }

    private SequenceTween ZoomAndRotate()
    {
        var duration = 1.5f;
        return
            new SequenceTween()
                .Add(
                    new CallbackTween(() =>
                    {
                        _wholeWord.Scale.Value = 10f;
                        _wholeWord.Angle.Value = MathF.PI / 2f;
                        _wholeWord.Opacity.Value = 0f;
                    }))
                .Add(
                    new MultiplexTween()
                        .AddChannel(new Tween<float>(_wholeWord.Scale, 1f, duration, Ease.QuadFastSlow))
                        .AddChannel(new Tween<float>(_wholeWord.Angle, 0f, duration, Ease.QuadFastSlow))
                        .AddChannel(new Tween<float>(_wholeWord.Opacity, 1f, duration, Ease.QuadFastSlow))
                )
            ;
    }

    private SequenceTween Tada()
    {
        var increment = 0.05f;

        return
            new SequenceTween()
                .Add(new Tween<float>(_wholeWord.Scale, 1.3f, 0.25f, Ease.QuadFastSlow))
                .Add(new Tween<float>(_wholeWord.Scale, 1.25f, 0.25f, Ease.QuadSlowFast))
                .Add(new MultiplexTween()
                    .AddChannel(new DynamicTween(() =>
                        {
                            var sequence = new SequenceTween();
                            var flip = 0;
                            var angleRange = 0.05f;
                            for (var i = 0f; i < 0.5f; i += increment)
                            {
                                flip++;
                                var even = flip % 2 == 0;
                                sequence.Add(new MultiplexTween()
                                    .AddChannel(new Tween<float>(_wholeWord.Angle, even ? angleRange : -angleRange,
                                        increment,
                                        Ease.Linear))
                                );
                            }

                            return sequence;
                        })
                    )
                )
                .Add(new MultiplexTween()
                    .AddChannel(new Tween<Vector2>(_wholeWord.Position,
                        new Vector2(0, 0), increment, Ease.Linear))
                    .AddChannel(new Tween<float>(_wholeWord.Angle, 0, increment,
                        Ease.Linear))
                )
                .Add(new WaitSecondsTween(0.05f))
                .Add(new Tween<float>(_wholeWord.Scale, 1f, 0.25f, Ease.QuadSlowFast))
            ;
    }

    private SequenceTween ZoomFade()
    {
        var increment = 0.05f;

        return
            new SequenceTween()
                .Add(new CallbackTween(() =>
                {
                    _wholeWord.Opacity.Value = 0f;
                    _wholeWord.Scale.Value = 3f;
                }))
                .Add(new MultiplexTween()
                    .AddChannel(
                        new SequenceTween()
                            .Add(new Tween<float>(_wholeWord.Scale, 0.9f, 0.5f, Ease.SineFastSlow))
                            .Add(new Tween<float>(_wholeWord.Scale, 1.25f, 0.15f, Ease.SineSlowFast))
                            .Add(new Tween<float>(_wholeWord.Scale, 1f, 0.15f, Ease.SineFastSlow))
                    )
                    .AddChannel(
                        new Tween<float>(_wholeWord.Opacity, 1f, 0.5f, Ease.Linear)
                    )
                )
                .Add(new MultiplexTween()
                    .AddChannel(new Tween<Vector2>(_wholeWord.Position,
                        new Vector2(0, 0), increment, Ease.Linear))
                    .AddChannel(new Tween<float>(_wholeWord.Angle, 0, increment,
                        Ease.Linear))
                )
                .Add(new WaitSecondsTween(0.05f))
                .Add(new Tween<float>(_wholeWord.Scale, 1f, 0.25f, Ease.QuadSlowFast))
            ;
    }

    private SequenceTween Ouch()
    {
        var duration = 1f;

        return
            new SequenceTween()
                .Add(new CallbackTween(() =>
                {
                    _wholeWord.Scale.Value = 1f;
                    _wholeWord.Opacity.Value = 1f;
                    _wholeWord.Position.Value = Vector2.Zero;
                }))
                .Add(
                    new MultiplexTween()
                        .AddChannel(
                            new Tween<float>(_wholeWord.Scale, 1.1f, duration, Ease.QuadSlowFast)
                        )
                        .AddChannel(new Tween<float>(_wholeWord.Opacity, 1f, duration, Ease.QuadSlowFast))
                        .AddChannel(
                            new SequenceTween()
                                .Add(new Tween<float>(_wholeWord.Angle, 1f, duration / 3, Ease.QuadFastSlow))
                                .Add(new Tween<float>(_wholeWord.Angle, -MathF.PI, duration / 3, Ease.QuadFastSlow))
                                .Add(new Tween<float>(_wholeWord.Angle, 0f, duration / 3, Ease.QuadFastSlow))
                        )
                        .AddChannel(
                            new SequenceTween()
                                .Add(new Tween<Vector2>(_wholeWord.Position, new Vector2(300, 500), duration / 3,
                                    Ease.QuadFastSlow))
                                .Add(new Tween<Vector2>(_wholeWord.Position, new Vector2(-300, -200), duration / 3,
                                    Ease.QuadFastSlow))
                                .Add(new Tween<Vector2>(_wholeWord.Position, Vector2.Zero, duration / 3,
                                    Ease.QuadSlowFast))
                        )
                )
                .Add(new Tween<float>(_wholeWord.Scale, 1f, 0.1f, Ease.QuadSlowFast))
            ;
    }

    private struct Figure
    {
        public TweenableFloat Angle { get; } = new();
        public TweenableVector2 Position { get; } = new();
        public TweenableFloat Scale { get; } = new(1);
        public TweenableFloat Opacity { get; } = new(1);

        public Figure(string text)
        {
            Text = text;
        }

        public string Text { get; }
    }
}
