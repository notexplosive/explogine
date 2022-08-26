using System;
using System.Collections.Generic;
using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SampleGame;

public class SampleGameCartridge : BasicGameCartridge
{
    private HitTestTarget _c3;
    private HitTestTarget _c2;
    private HitTestTarget _c1;

    public override void OnCartridgeStarted()
    {
        Client.Debug.Log("Sample Cart Loaded");
        
        _c1 = new HitTestTarget(new Rectangle(100, 100, 500, 500), new Depth(50));
        _c2 = new HitTestTarget(new Rectangle(150, 150, 500, 500), new Depth(25));
        _c3 = new HitTestTarget(new Rectangle(200, 200, 500, 500), new Depth(15));
    }

    public override void Update(float dt)
    {
        
        if (Client.Input.Keyboard.GetButton(Keys.F).WasPressed)
        {
            Client.Window.SetFullscreen(!Client.Window.IsFullscreen);
        }
        
        Client.HitTesting.Add(_c1);
        Client.HitTesting.Add(_c2);
        Client.HitTesting.Add(_c3);

        if (Client.Input.Keyboard.GetButton(Keys.Space).WasPressed)
        {
            Client.Debug.Log(DateTime.UtcNow.Second);
            Client.Debug.Log("very\n very\n very\n very long message");
        }
    }

    public override void Draw(Painter painter)
    {
        painter.Clear(Color.CornflowerBlue);
        painter.BeginSpriteBatch(SamplerState.PointWrap);

        void DrawCollider(HitTestTarget collider, Color color)
        {
            if (Client.HitTesting.LastTopHit == collider)
            {
                color = Color.Blue;
            }
            painter.DrawAsRectangle(Client.Assets.GetTexture("white-pixel"), collider.Rectangle,
                new DrawSettings {Color = color, Depth = collider.Depth});
        }

        DrawCollider(_c1, Color.SeaGreen);
        DrawCollider(_c2, Color.Aqua);
        DrawCollider(_c3, Color.Coral);
        
        painter.EndSpriteBatch();
    }

    public override void SetupFormalParameters(ParsedCommandLineArguments args)
    {
        args.RegisterParameter<int>("level");
    }

    public override IEnumerable<LoadEvent> LoadEvents(Painter painter)
    {
        yield return () =>
        {
            var canvas = new Canvas(1, 1);
            Client.Graphics.PushCanvas(canvas);

            painter.BeginSpriteBatch(SamplerState.PointWrap);
            painter.Clear(Color.White);
            painter.EndSpriteBatch();

            Client.Graphics.PopCanvas();

            return canvas.AsAsset("white-pixel");
        };
    }
}
