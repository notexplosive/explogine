using System;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Rails;
using Microsoft.Xna.Framework;

namespace ExplogineMonoGame.Debugging;

public class FramerateCounter : IUpdateHook, IDrawHook
{
    private float _framerate;
    private DateTime _previousFrame;

    public void Draw(Painter painter)
    {
        if (_framerate > 0)
        {
            painter.BeginSpriteBatch();
            painter.DrawDebugStringAtPosition(_framerate.ToString("F0"), Vector2.Zero, new DrawSettings());
            painter.EndSpriteBatch();
        }
    }

    public void Update(float dt)
    {
        var timeSinceLastFrame = DateTime.Now - _previousFrame;
        _framerate = 1f / (float)timeSinceLastFrame.TotalSeconds;

        _previousFrame = DateTime.Now;
    }
}
