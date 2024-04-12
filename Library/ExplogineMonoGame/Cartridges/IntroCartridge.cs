using System;
using System.Collections.Generic;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExTween;
using ExTweenMonoGame;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Cartridges;

public class IntroCartridge : Cartridge
{
    private readonly uint _index;
    private readonly List<Figure> _letters = new();
    private readonly IndirectFont _logoFont = new("engine/logo-font", 128);
    private readonly string _text;
    private bool _cancelEarly;
    private float _startingDelay;
    private bool _useWholeWord = true;
    private Figure _wholeWord;

    public IntroCartridge(IRuntime runtime, string text, uint index, float startingDelay = 0f) : base(runtime)
    {
        _text = text;
        _index = index;
        _startingDelay = startingDelay;
    }

    public SequenceTween Tween { get; } = new();

    public override CartridgeConfig CartridgeConfig { get; } = new();

    public override void OnCartridgeStarted()
    {
        _wholeWord = new Figure(_text);

        foreach (var character in _text)
        {
            _letters.Add(new Figure(character.ToString()));
        }

        var tweens = new[]
        {
            HeartBeat,
            RingRing,
            // Ouch,
            Ohm,
            FlyInLetters,
            FlyInLettersRandom
        };

        Tween.Add(tweens[_index % tweens.Length]());
        Tween.Add(new WaitSecondsTween(2f));
    }

    public override void Update(float dt)
    {
        _startingDelay -= dt;
        if (_startingDelay > 0)
        {
            return;
        }

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

    public override void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(
            Matrix.CreateTranslation(new Vector3(-Runtime.Window.RenderResolution.ToVector2() / 2, 0))
            * Matrix.CreateScale(new Vector3(new Vector2(_wholeWord.Scale), 1))
            * Matrix.CreateTranslation(new Vector3(Runtime.Window.RenderResolution.ToVector2() / 2, 0))
        );
        painter.Clear(Color.Navy);

        var centerOfScreen = Runtime.Window.RenderResolution.ToVector2() / 2;

        if (_startingDelay <= 0)
        {
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
        }

        painter.EndSpriteBatch();
    }

    public override void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        if (input.Keyboard.IsAnyKeyDown() || input.Mouse.WasAnyButtonPressedOrReleased())
        {
            Tween.SkipToEnd();
        }
    }

    public override bool ShouldLoadNextCartridge()
    {
        return Tween.IsDone() || _cancelEarly;
    }

    public override void Unload()
    {
    }

    private SequenceTween FlyInLetters()
    {
        _useWholeWord = false;

        var durationPerLetter = 0.5f;

        var pitch = 0.5f;
        var pitchIncrement = 1f / _letters.Count / 2f;

        float CurrentPitch()
        {
            var result = pitch;
            pitch += pitchIncrement;
            return result;
        }

        var result = new SequenceTween();
        result.Add(new CallbackTween(() =>
        {
            foreach (var letter in _letters)
            {
                letter.Scale.Value = 2f;
                letter.Opacity.Value = 0f;
            }

            // reset pitch
            pitch = 0.5f;
        }));
        result.Add(new DynamicTween(() =>
            {
                var index = 0;
                var multi = new MultiplexTween();
                foreach (var letter in _letters)
                {
                    multi.Add(new SequenceTween()
                        .Add(new WaitSecondsTween(index * 0.05f))
                        .Add(new CallbackTween(() =>
                            Client.SoundPlayer.Play("engine/click", new SoundEffectSettings {Pitch = CurrentPitch()})))
                        .Add(new MultiplexTween()
                            .Add(
                                new SequenceTween()
                                    .Add(new Tween<float>(letter.Scale, 0.95f, durationPerLetter * 3 / 4,
                                        Ease.QuadFastSlow))
                                    .Add(new Tween<float>(letter.Scale, 1f, durationPerLetter * 1 / 4,
                                        Ease.QuadSlowFast))
                            )
                            .Add(new Tween<float>(letter.Opacity, 1f, durationPerLetter, Ease.Linear)))
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
                    multi.Add(new SequenceTween()
                        .Add(new WaitSecondsTween(Client.Random.Dirty.NextFloat()))
                        .Add(new MultiplexTween()
                            .Add(
                                new SequenceTween()
                                    .Add(new Tween<float>(letter.Scale, 0.95f, durationPerLetter * 3 / 4,
                                        Ease.QuadFastSlow))
                                    .Add(new CallbackTween(() => Client.SoundPlayer.Play("engine/bubbles",
                                        new SoundEffectSettings
                                            {Pitch = Client.Random.Dirty.NextFloat(0.5f, 1f), Volume = 1f})))
                                    .Add(new Tween<float>(letter.Scale, 1f, durationPerLetter * 1 / 4,
                                        Ease.QuadSlowFast))
                            )
                            .Add(new Tween<float>(letter.Opacity, 1f, durationPerLetter, Ease.Linear)))
                    );
                }

                return multi;
            }))
            .Add(new WaitSecondsTween(0.25f))
            ;

        return result;
    }

    private SequenceTween Ohm()
    {
        var duration = 1.25f;
        return
            new SequenceTween()
                .Add(
                    new CallbackTween(() =>
                    {
                        // _wholeWord.Scale.Value = 10f;
                        // _wholeWord.Angle.Value = MathF.PI / 2f;
                        _wholeWord.Opacity.Value = 0f;
                    }))
                .Add(new CallbackTween(() => Client.SoundPlayer.Play("engine/ohm", new SoundEffectSettings())))
                .Add(new WaitSecondsTween(0.25f))
                .Add(
                    new MultiplexTween()
                        .Add(
                            new SequenceTween()
                                .Add(new Tween<float>(_wholeWord.Scale, 1.1f, duration * 1 / 4f, Ease.QuadFastSlow))
                                .Add(new Tween<float>(_wholeWord.Scale, 1.4f, duration * 1 / 4f, Ease.QuadFastSlow))
                                .Add(new Tween<float>(_wholeWord.Scale, 1f, duration / 2, Ease.QuadSlowFast))
                        )
                        // .Add(new Tween<float>(_wholeWord.Angle, 0f, duration, Ease.QuadFastSlow))
                        .Add(new Tween<float>(_wholeWord.Opacity, 1f, duration, Ease.Linear))
                )
            ;
    }

    private SequenceTween RingRing()
    {
        var increment = 0.05f;

        return
            new SequenceTween()
                .Add(new CallbackTween(() =>
                    Client.SoundPlayer.Play("engine/clink", new SoundEffectSettings {Pitch = -0.5f})))
                .Add(new Tween<float>(_wholeWord.Scale, 1.3f, 0.25f, Ease.QuadFastSlow))
                .Add(new Tween<float>(_wholeWord.Scale, 1.25f, 0.25f, Ease.QuadSlowFast))
                .Add(new DynamicTween(() =>
                    {
                        var sequence = new SequenceTween();
                        var flip = 0;
                        var angleRange = 0.05f;
                        for (var i = 0f; i < 0.5f; i += increment)
                        {
                            flip++;
                            var even = flip % 2 == 0;
                            sequence.Add(new MultiplexTween()
                                .Add(new Tween<float>(_wholeWord.Angle, even ? angleRange : -angleRange,
                                    increment,
                                    Ease.Linear))
                                .Add(new CallbackTween(() =>
                                    Client.SoundPlayer.Play("engine/clink", new SoundEffectSettings {Pitch = 0.5f})))
                            );
                        }

                        return sequence;
                    })
                )
                .Add(new MultiplexTween()
                    .Add(new Tween<Vector2>(_wholeWord.Position,
                        new Vector2(0, 0), increment, Ease.Linear))
                    .Add(new Tween<float>(_wholeWord.Angle, 0, increment,
                        Ease.Linear))
                )
                .Add(new WaitSecondsTween(0.05f))
                .Add(new Tween<float>(_wholeWord.Scale, 1f, 0.25f, Ease.QuadSlowFast))
                .Add(new CallbackTween(() => Client.SoundPlayer.Play("engine/clink", new SoundEffectSettings())))
            ;
    }

    private SequenceTween HeartBeat()
    {
        var increment = 0.05f;

        return
            new SequenceTween()
                .Add(new CallbackTween(() => { _wholeWord.Opacity.Value = 0f; }))
                .Add(new WaitSecondsTween(0.25f))
                .Add(new CallbackTween(() =>
                {
                    _wholeWord.Opacity.Value = 1f;
                    _wholeWord.Scale.Value = 1f;
                }))
                .Add(new MultiplexTween()
                    .Add(
                        new SequenceTween()
                            .Add(new CallbackTween(() =>
                                Client.SoundPlayer.Play("engine/jar1", new SoundEffectSettings())))
                            .Add(new Tween<float>(_wholeWord.Scale, 1.25f, 0.15f, Ease.SineSlowFast))
                            .Add(new CallbackTween(() =>
                                Client.SoundPlayer.Play("engine/jar2", new SoundEffectSettings())))
                            .Add(new Tween<float>(_wholeWord.Scale, 1f, 0.15f, Ease.SineFastSlow))
                    )
                    .Add(
                        new Tween<float>(_wholeWord.Opacity, 1f, 0.5f, Ease.Linear)
                    )
                )
                .Add(new MultiplexTween()
                    .Add(new Tween<Vector2>(_wholeWord.Position,
                        new Vector2(0, 0), increment, Ease.Linear))
                    .Add(new Tween<float>(_wholeWord.Angle, 0, increment,
                        Ease.Linear))
                )
                .Add(new WaitSecondsTween(0.05f))
                .Add(new Tween<float>(_wholeWord.Scale, 1f, 0.25f, Ease.QuadSlowFast))
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
