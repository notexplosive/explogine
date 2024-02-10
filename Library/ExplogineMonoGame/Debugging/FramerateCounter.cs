using System;
using System.Diagnostics;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Debugging;

public class FramerateCounter : IUpdateHook, IDrawHook
{
    private float _updatesPerSecond;
    private DateTime _previousUpdate;
    private DateTime _previousDraw;

    public void Draw(Painter painter)
    {
        var timeSinceLastDraw = DateTime.Now - _previousDraw;
        float drawsPerSecond = 1f / (float)timeSinceLastDraw.TotalSeconds;
        _previousDraw = DateTime.Now;
        
        if (_updatesPerSecond > 0)
        {
            var memoryUsageStat = string.Empty;

            if (Client.Debug.MonitorMemoryUsage)
            {
                var currentProc = Process.GetCurrentProcess();
                long memoryUsed = currentProc.PrivateMemorySize64;
                memoryUsageStat = (memoryUsed / 1000_000f).ToString("F2") + " MB";
            }
            
            painter.BeginSpriteBatch();
            painter.DrawDebugStringAtPosition($"{memoryUsageStat} {_updatesPerSecond:F0} UPS / {drawsPerSecond:F0} FPS" , Vector2.Zero, new DrawSettings());
            painter.EndSpriteBatch();
        }
    }

    public void Update(float dt)
    {
        var timeSinceLastDraw = DateTime.Now - _previousUpdate;
        _updatesPerSecond = 1f / (float)timeSinceLastDraw.TotalSeconds;
        _previousUpdate = DateTime.Now;
    }
}
