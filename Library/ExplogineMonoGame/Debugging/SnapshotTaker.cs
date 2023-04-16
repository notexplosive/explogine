using System;
using System.IO;
using System.Reflection;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using ExTween;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

internal class SnapshotTaker : IUpdateInputHook, IUpdateHook
{
    private readonly IRuntime _runtime;
    private DateTime _timeLastFrame;
    private float _timer = 0.1f;
    private float _timerMax = 2f;
    private bool _timerReady;
    private ITween _tween = new EmptyTween();

    public SnapshotTaker(IRuntime runtime)
    {
        _runtime = runtime;
    }

    public void Update(float dt)
    {
        _tween.Update(dt);
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        var now = DateTime.Now;
        if (input.Keyboard.GetButton(Keys.F12).WasPressed)
        {
            TakeSnapshot();
        }

        if (_timerReady && Client.Runtime.Window.IsInFocus)
        {
            _timer -= (float) (now - _timeLastFrame).TotalSeconds;
            if (_timer < 0)
            {
                _tween = new SequenceTween()
                        .Add(new CallbackTween(() => { Client.Debug.Log("Taking snapshot..."); }))
                        .Add(new WaitSecondsTween(0.1f))
                        .Add(new CallbackTween(TakeSnapshot))
                    ;
                _timerMax = Math.Min(30, _timerMax * 2);
                _timer = _timerMax;
            }
        }

        _timeLastFrame = now;
    }

    public void StartTimer()
    {
        _timerReady = true;
        _timeLastFrame = DateTime.Now;
    }

    private void TakeSnapshot()
    {
        var currentTime = DateTime.Now;
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var directory = Path.Join(homeDirectory, "Screenshots", Assembly.GetEntryAssembly()!.GetName().Name);
        Directory.CreateDirectory(directory);
        var screenshotFilePath = Path.Join(directory, $"{currentTime.ToFileTimeUtc()}.png");
        using var stream = File.Create(screenshotFilePath);
        var texture = _runtime.Window.Canvas.Texture;
        texture.SaveAsPng(stream, texture.Width, texture.Height);
        Client.Debug.Log("Snapshot:", screenshotFilePath);
    }
}
