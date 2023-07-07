namespace ExplogineMonoGame.Rails;

public interface IEarlyDrawHook : IHook
{
    void EarlyDraw(Painter painter);
}
