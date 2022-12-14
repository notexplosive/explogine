using ExplogineMonoGame;
using Microsoft.Xna.Framework;

namespace MachinaLite;

public class Camera
{
    public Matrix ScreenToWorldMatrix =>
        Matrix.CreateTranslation(new Vector3(Position, 0)) *
        Matrix.CreateScale(new Vector3(new Vector2(Scale, Scale), 1));

    public Matrix WorldToScreenMatrix => Matrix.Invert(ScreenToWorldMatrix);
    public Vector2 Position { get; set; }
    public float Scale { get; set; } = 1f;

    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, ScreenToWorldMatrix);
    }
    
    public Vector2 WorldToScreen(Vector2 worldPosition)
    {
        return Vector2.Transform(worldPosition, Matrix.Invert(ScreenToWorldMatrix));
    }

    public Rectangle ViewRectInWorldSpace
    {
        get
        {
            var position = ScreenToWorld(Vector2.Zero);
            var size = Client.Window.RenderResolution.ToVector2() * Scale;

            return new Rectangle(position.ToPoint(), size.ToPoint());
        }
    }
}
