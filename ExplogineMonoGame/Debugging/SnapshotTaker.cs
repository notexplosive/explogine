using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

internal class SnapshotTaker
{
    private float _timer = 0.1f;
    private float _timerMax = 2f;
    private bool _timerReady;

    public void StartTimer()
    {
        _timerReady = true;
    }

    public void Update(float dt)
    {
        if (Client.Input.Keyboard.GetButton(Keys.F12).WasPressed)
        {
            TakeSnapshot();
        }

        if (_timerReady)
        {
            _timer -= dt;
            if (_timer < 0)
            {
                TakeSnapshot();
                _timerMax = Math.Min(30, _timerMax * 2);
                _timer = _timerMax;
            }
        }
    }

    private void TakeSnapshot()
    {
        var currentTime = DateTime.Now;
        var homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var directory = Path.Join(homeDirectory, "screenshots", Assembly.GetEntryAssembly()!.GetName().Name);
        Directory.CreateDirectory(directory);
        var screenshotFilePath = Path.Join(directory, $"{currentTime.ToFileTimeUtc()}.png");
        using var stream = File.Create(screenshotFilePath);
        var texture = Client.RenderCanvas.Canvas.Texture;
        texture.SaveAsPng(stream, texture.Width, texture.Height);
        Client.Debug.Log("Snapshot:", screenshotFilePath);
    }
}
