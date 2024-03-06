﻿using System;
using System.Diagnostics;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Debugging;

public class FramerateCounter : IUpdateHook, IDrawHook
{
    private DateTime _previousDraw;

    public void Draw(Painter painter)
    {
        var timeSinceLastDraw = DateTime.Now - _previousDraw;
        float drawDuration = (float)timeSinceLastDraw.TotalSeconds;
        _previousDraw = DateTime.Now;
        
        var memoryUsageStat = string.Empty;

        if (Client.Debug.MonitorMemoryUsage)
        {
            var currentProc = Process.GetCurrentProcess();
            long memoryUsed = currentProc.PrivateMemorySize64;
            memoryUsageStat = (memoryUsed / 1000_000f).ToString("F2") + " MB ";
        }

        painter.BeginSpriteBatch();
        painter.DrawDebugStringAtPosition($"{memoryUsageStat}{drawDuration:F4}ms {1 / drawDuration:F0}" , Vector2.Zero, new DrawSettings{Color = Color.White});
        painter.EndSpriteBatch();
    }

    public void Update(float dt)
    {
    }
}
