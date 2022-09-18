using ExplogineMonoGame;

namespace Machina;

public class Scene
{
    private readonly List<Actor> _actors = new();
    public bool Frozen { get; set; } = false;

    public void Update(float dt)
    {
        if (Frozen)
        {
            return;
        }

        foreach (var actor in _actors)
        {
            foreach (var component in actor.AllComponents())
            {
                component.Update(dt);
            }
        }
    }

    public void Draw(Painter painter)
    {
        foreach (var actor in _actors)
        {
            if (actor.Visible)
            {
                foreach (var component in actor.AllComponents())
                {
                    component.Draw(painter);
                }
            }
        }
    }

    public Actor AddActor()
    {
        var actor = new Actor();
        _actors.Add(actor);
        return actor;
    }
}
