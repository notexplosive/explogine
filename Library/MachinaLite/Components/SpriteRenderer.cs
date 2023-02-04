using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.AssetManagement;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace MachinaLite.Components;

public class SpriteRenderer : BaseComponent
{
    private IFrameAnimation _currentAnimation;
    private float _elapsedTime;

    public SpriteRenderer(Actor actor, SpriteSheet spriteSheet) : base(actor)
    {
        SpriteSheet = spriteSheet;
        _currentAnimation = spriteSheet.DefaultAnimation;
        Color = Color.White;
    }

    public Vector2 Offset { get; set; }
    public Scale2D Scale { get; set; } = Scale2D.One;
    public SpriteSheet SpriteSheet { get; }
    public Color Color { get; set; }
    public int FramesPerSecond { get; set; } = 15;
    public bool IsPaused { get; set; }
    public XyBool Flip { get; set; } = XyBool.False;
    public int CurrentFrame => _currentAnimation.GetFrame(_elapsedTime);

    public SpriteRenderer SetupBox()
    {
        var box = Actor.GetComponent<Box>();
        var gridBasedSpriteSheet = SpriteSheet as GridBasedSpriteSheet;

        if (gridBasedSpriteSheet == null)
        {
            throw new Exception(
                $"{nameof(SpriteRenderer.SetupBox)}() failed because {nameof(SpriteRenderer.SpriteSheet)} was not a {nameof(GridBasedSpriteSheet)}");
        }

        box ??= new Box(Actor, Point.Zero, Point.Zero);

        var size = new Point
        {
            X = (int) (gridBasedSpriteSheet.FrameSize.X * Scale.Value.X),
            Y = (int) (gridBasedSpriteSheet.FrameSize.Y * Scale.Value.Y)
        };
        box.Size = size;

        box.SetOffsetToCenter();

        return this;
    }

    public override void Draw(Painter painter)
    {
        SpriteSheet.DrawFrameAtPosition(painter, CurrentFrame, Actor.Transform.Position - Offset,
            Scale, new DrawSettings
            {
                Depth = Actor.Transform.Depth,
                Color = Color,
                Flip = Flip,
                Angle = Actor.Transform.Angle
            });
    }

    public override void Update(float dt)
    {
        if (!IsPaused)
        {
            IncrementTime(dt);
        }
    }

    public void SetFrame(int frame)
    {
        _elapsedTime = frame;
    }

    public bool IsAnimationFinished()
    {
        return _elapsedTime > _currentAnimation.Length;
    }

    public SpriteRenderer SetAnimation(IFrameAnimation animation)
    {
        if (!_currentAnimation.Equals(animation))
        {
            _elapsedTime = 0;
            _currentAnimation = animation;
        }

        return this;
    }

    private void IncrementTime(float dt)
    {
        SetElapsedTime(_elapsedTime + dt * FramesPerSecond);
    }

    private void SetElapsedTime(float newTime)
    {
        _elapsedTime = newTime;
    }
}
