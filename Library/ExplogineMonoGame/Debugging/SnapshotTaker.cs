using System;
using System.IO;
using System.Reflection;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

internal class SnapshotTaker
{
    private float _timer = 0.1f;
    private float _timerMax = 2f;
    private bool _timerReady;
    private DateTime _timeLastFrame;

    public void StartTimer()
    {
        _timerReady = true;
        _timeLastFrame = DateTime.Now;
    }

    public void Update(InputFrameState input)
    {
        var now = DateTime.Now;
        if (input.Keyboard.GetButton(Keys.F12).WasPressed)
        {
            TakeSnapshot();
        }
        
        if (_timerReady)
        {
            _timer -= (float)(now - _timeLastFrame).TotalSeconds;
            if (_timer < 0)
            {
                TakeSnapshot();
                _timerMax = Math.Min(30, _timerMax * 2);
                _timer = _timerMax;
            }
        }
        
        _timeLastFrame = now;
    }

    private void TakeSnapshot()
    {
        var currentTime = DateTime.Now;
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var directory = Path.Join(homeDirectory, "Screenshots", Assembly.GetEntryAssembly()!.GetName().Name);
        Directory.CreateDirectory(directory);
        var screenshotFilePath = Path.Join(directory, $"{currentTime.ToFileTimeUtc()}.png");
        using var stream = File.Create(screenshotFilePath);
        var texture = Client.ClientCanvas.Canvas.Texture;
        texture.SaveAsPng(stream, texture.Width, texture.Height);
        Client.Debug.Log("Snapshot:", screenshotFilePath);
    }
}
