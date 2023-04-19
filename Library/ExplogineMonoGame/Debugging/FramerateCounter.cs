using System;
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
            painter.BeginSpriteBatch();
            painter.DrawDebugStringAtPosition($"{_updatesPerSecond:F0} UPS / {drawsPerSecond:F0} FPS" , Vector2.Zero, new DrawSettings());
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
