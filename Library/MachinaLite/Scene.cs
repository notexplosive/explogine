using ExplogineCore;
using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MachinaLite;

public class Scene : Crane<Actor>
{
    private readonly List<CoroutineWrapper> _coroutines = new();
    public readonly MachCamera MachCamera;
    private readonly List<Action> _deferredActions = new();

    public bool IsFrozen { get; private set; }

    public float TimeScale { get; set; } = 1f;

    public Scene(IRuntime runtime)
    {
        MachCamera = new MachCamera(runtime);
    }
    
    public Actor AddActor(string name, Vector2 position = new(), float angle = 0f,
        int depthAsInt = Depth.MaxAsInt / 2)
    {
        var actor = new Actor(name, this);
        actor.Transform.Position = position;
        actor.Transform.Angle = angle;
        actor.Transform.Depth = new Depth(depthAsInt);
        Iterables.Add(actor);
        return actor;
    }

    public WaitUntil StartCoroutine(IEnumerator<ICoroutineAction> coroutine)
    {
        var wrapper = new CoroutineWrapper(coroutine);
        _coroutines.Add(wrapper);
        coroutine.MoveNext();
        return new WaitUntil(wrapper.IsDone);
    }

    public List<Actor> GetRootLevelActors()
    {
        return new List<Actor>(Iterables);
    }

    public List<Actor> GetAllActors()
    {
        void ExtractChild(List<Actor> accumulator, Actor actor)
        {
            if (IsIterablePendingDeletion(actor))
            {
                return;
            }

            accumulator.Add(actor);

            for (var i = 0; i < actor.Transform.ChildCount; i++)
            {
                var child = actor.Transform.ChildAt(i);
                if (!actor.Transform.IsIterablePendingDeletion(child))
                {
                    ExtractChild(accumulator, child!);
                }
            }
        }

        var result = new List<Actor>();
        foreach (var actor in Iterables)
        {
            ExtractChild(result, actor);
        }

        return result;
    }

    public Actor AddActor(Actor actor)
    {
        AddIterable(actor);
        return actor;
    }

    public void DeleteActor(Actor actor)
    {
        if (actor.Transform.Parent != null)
        {
            actor.Transform.Parent.DeleteChild(actor);
        }
        else
        {
            DeleteIterable(actor);
        }
    }

    public void GentlyRemoveActor(Actor actor)
    {
        GentlyRemoveIterable(actor);
    }

    public override void Update(float dt)
    {
        foreach (var action in _deferredActions)
        {
            action.Invoke();
        }

        _deferredActions.Clear();

        var coroutinesCopy = new List<CoroutineWrapper>(_coroutines);
        foreach (var coroutine in coroutinesCopy)
        {
            if (coroutine.Current == null)
            {
                _coroutines.Remove(coroutine);
            }
            else if (coroutine.Current.IsComplete(dt * TimeScale))
            {
                var hasNext = coroutine.MoveNext();
                if (!hasNext || coroutine.Current == null)
                {
                    _coroutines.Remove(coroutine);
                    coroutine.Dispose();
                }
            }
        }

        base.Update(dt * TimeScale);
    }

    /// <summary>
    ///     Scene stops getting input, stops updating, it just draws
    /// </summary>
    public void Freeze()
    {
        IsFrozen = true;
    }

    public void Unfreeze()
    {
        IsFrozen = false;
    }

    public override void Draw(Painter painter)
    {
        painter.BeginSpriteBatch(MachCamera.WorldToScreenMatrix);

        base.Draw(painter);

        painter.EndSpriteBatch();
    }

    public override void DebugDraw(Painter painter)
    {
        painter.BeginSpriteBatch(MachCamera.WorldToScreenMatrix);

        base.DebugDraw(painter);

        painter.EndSpriteBatch();
    }

    public override void OnMouseButton(MouseButton mouseButton, Vector2 screenPosition, ButtonState buttonState,
        HitTestStack hitTestStack)
    {
        // Convert position to account for camera
        base.OnMouseButton(mouseButton, MachCamera.ScreenToWorld(screenPosition), buttonState, hitTestStack);
    }

    public override void OnMouseUpdate(Vector2 screenPosition, Vector2 worldDelta, Vector2 rawDelta,
        HitTestStack hitTestStack)
    {
        // Convert position to account for camera
        base.OnMouseUpdate(MachCamera.ScreenToWorld(screenPosition), worldDelta, rawDelta, hitTestStack);
    }

    public int CountActors()
    {
        return Iterables.Count;
    }

    ~Scene()
    {
        DeleteAllActors();
    }

    public void DeleteAllActors()
    {
        foreach (var actor in GetAllActors())
        {
            actor.Delete();
        }

        FlushBuffers();
    }

    /// <summary>
    ///     Queues up an arbitrary function to be run at the start of next Update()
    /// </summary>
    /// <param name="action"></param>
    public void AddDeferredAction(Action action)
    {
        _deferredActions.Add(action);
    }
}
