using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Debugging;

public class FramerateCounter : IUpdateHook, IDrawHook
{
    private DateTime _previousDraw;
    private Queue<float> _savedDurations = new();

    public void Draw(Painter painter)
    {
        var timeSinceLastDraw = DateTime.Now - _previousDraw;
        var drawDuration = (float) timeSinceLastDraw.TotalSeconds;
        _previousDraw = DateTime.Now;

        var memoryUsageStat = string.Empty;

        if (Client.Debug.MonitorMemoryUsage)
        {
            var currentProc = Process.GetCurrentProcess();
            var memoryUsed = currentProc.PrivateMemorySize64;
            memoryUsageStat = (memoryUsed / 1000_000f).ToString("F2") + " MB ";
        }

        var finalString = $"{memoryUsageStat}{drawDuration:F4}s {1 / drawDuration:F0}";

        _savedDurations.Enqueue(drawDuration);

        while (_savedDurations.Count > 60)
        {
            _savedDurations.Dequeue();
        }

        var high = _savedDurations.First();
        var low = _savedDurations.First();
        var avg = 0f;
        foreach (var duration in _savedDurations)
        {
            high = Math.Max(high, duration);
            low = Math.Min(low, duration);
            avg += duration;
        }

        avg /= _savedDurations.Count;

        finalString += $" (avg: {1/avg:F0}, max: {1/low:F0}, min: {1/high:F0})";

        painter.BeginSpriteBatch();
        painter.DrawStringWithinRectangle(Client.Assets.GetFont("engine/console-font", 32), finalString,
            Client.Runtime.Window.Size.ToRectangleF(), Alignment.BottomLeft, new DrawSettings {Color = Color.White});
        painter.EndSpriteBatch();
    }

    public void Update(float dt)
    {
    }
}
