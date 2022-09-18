using ExplogineMonoGame;

namespace Machina;

public abstract class Component
{
    public abstract void Update(float dt);
    public abstract void Draw(Painter painter);
}
