using ExplogineCore.Data;
using ExplogineMonoGame;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;

namespace MachinaLite;

public class Transform : Crane<Actor>, IComponent
{
    public readonly Actor Actor;
    private float _angleImpl;

    private Depth _depthImpl = Depth.Middle;
    private float _localAngleImpl;
    private Depth _localDepthImpl;
    private Vector2 _localPositionImpl;
    private Vector2 _positionImpl;

    public Transform(Actor actor)
    {
        Actor = actor;
    }

    public Transform? Parent { get; private set; }

    public Depth Depth
    {
        get => _depthImpl;
        set
        {
            _depthImpl = value;
            for (var i = 0; i < ChildCount; i++)
            {
                var child = ChildAt(i);
                child!.Transform.Depth = _depthImpl + child.Transform._localDepthImpl;
            }
        }
    }

    public Depth LocalDepth
    {
        get
        {
            if (HasParent)
            {
                return _localDepthImpl;
            }

            return _depthImpl;
        }

        set
        {
            if (HasParent)
            {
                _localDepthImpl = value;
                Depth = Parent!.Depth + _localDepthImpl;
            }
            else
            {
                Depth = value;
            }
        }
    }

    public float Angle
    {
        get => _angleImpl;
        set
        {
            if (float.IsNaN(value))
            {
                return;
            }

            _angleImpl = value;
            for (var i = 0; i < ChildCount; i++)
            {
                var child = ChildAt(i);
                child!.Transform.Angle = _angleImpl + child.Transform._localAngleImpl;
                child.Transform.Position = child.Transform.LocalToWorldPosition(child.Transform.LocalPosition);
            }
        }
    }

    public float LocalAngle
    {
        get
        {
            if (HasParent)
            {
                return _localAngleImpl;
            }

            return Angle;
        }

        set
        {
            if (HasParent)
            {
                _localAngleImpl = value;
                Angle = Parent!.Angle + _localAngleImpl;
            }
            else
            {
                Angle = value;
            }
        }
    }

    public Vector2 Position
    {
        get => _positionImpl;
        set
        {
            _positionImpl = value;
            _localPositionImpl = WorldToLocalPosition(_positionImpl);
            for (var i = 0; i < ChildCount; i++)
            {
                var child = ChildAt(i);
                child!.Transform.Position = child.Transform.LocalToWorldPosition(child.Transform._localPositionImpl);
            }
        }
    }

    public Vector2 LocalPosition
    {
        get
        {
            if (HasParent)
            {
                return _localPositionImpl;
            }

            return Position;
        }

        set
        {
            if (HasParent)
            {
                _localPositionImpl = value;
                Position = LocalToWorldPosition(_localPositionImpl);
            }
            else
            {
                Position = value;
            }
        }
    }

    public int ChildCount => Iterables.Count;

    public Matrix TransformMatrix
    {
        get
        {
            if (HasParent)
            {
                var pos = Parent!.Position;
                return Matrix.CreateRotationZ(Parent._angleImpl)
                       * Matrix.CreateTranslation(pos.X, pos.Y, 0);
            }

            return Matrix.Identity;
        }
    }

    public bool HasParent => Parent != null;

    public override void Update(float dt)
    {
        // Transform needs to call base if we insert any code into Update()
        base.Update(dt);
    }

    public override void DebugDraw(Painter painter)
    {
        /*
        painter.DrawLine(Position, Position + new Angle(-Angle).ToUnitVector() * 15, Color.LawnGreen, 2, Depth);
        painter.DrawLine(Position, LocalToWorldPosition(LocalPosition + new Vector2(15, 0)), Color.Cyan, 2,
            Depth);
        painter.DrawLine(Position, LocalToWorldPosition(LocalPosition + new Vector2(0, -15)), Color.OrangeRed,
            2, Depth);

        if (HasParent)
        {
            painter.SpriteBatch.DrawLine(Position, Parent.Position, Color.White, 1, Depth);
        }
        */

        painter.DrawRectangle(new Rectangle(Position.ToPoint() - new Point(5), new Point(10)),
            new DrawSettings {Color = Color.Aqua});

        // Transform needs to call base
        base.DebugDraw(painter);
    }

    public void OnActorDestroy()
    {
        foreach (var iterable in Iterables)
        {
            DeleteIterable(iterable);
        }
    }

    public int GetChildIndex(Transform transform)
    {
        return Iterables.IndexOf(transform.Actor);
    }

    public void DeleteChild(Actor actor)
    {
        DeleteIterable(actor);
    }

    public void SetParent(Actor? newParent)
    {
        if (newParent == Actor)
        {
            return;
        }

        if (HasParent)
        {
            Parent!.RemoveChild(Actor);
        }

        if (newParent != null)
        {
            Parent = newParent.Transform;
            newParent.Transform.AddChild(Actor);
            LocalPosition = WorldToLocalPosition(Position);
            LocalAngle = Angle - newParent.Transform.Angle;
            LocalDepth = Depth - newParent.Transform.Depth;
        }
        else
        {
            Parent = null;
        }
    }

    /// <summary>
    ///     Remove from scene and add to hierarchy
    /// </summary>
    /// <param name="child"></param>
    private void AddChild(Actor child)
    {
        // If the actor is in a scene, remove them
        child.Scene?.GentlyRemoveActor(child);
        AddIterable(child);
    }

    /// <summary>
    ///     Remove from hierarchy and re-add to scene
    /// </summary>
    /// <param name="child"></param>
    private void RemoveChild(Actor child)
    {
        GentlyRemoveIterable(child);
        child.Transform.Parent = null;
        child.Scene?.AddActor(child);
    }

    public bool HasChildAt(int index)
    {
        return index >= 0 && index < Iterables.Count;
    }

    public Actor? ChildAt(int index)
    {
        if (HasChildAt(index))
        {
            return Iterables[index];
        }

        return null;
    }

    public Vector2 LocalToWorldPosition(Vector2 localPos)
    {
        return Vector2.Transform(localPos, TransformMatrix);
    }

    public Vector2 WorldToLocalPosition(Vector2 worldPos)
    {
        return Vector2.Transform(worldPos, Matrix.Invert(TransformMatrix));
    }

    public Actor AddActorAsChild(string name, Vector2 localPosition = default)
    {
        var newActor = Actor.Scene.AddActor(name);

        newActor.Transform.SetParent(Actor);
        newActor.Transform.LocalPosition = localPosition;
        newActor.Transform.LocalDepth = new Depth(0);
        newActor.Transform.LocalAngle = 0f;

        FlushBuffers();
        return newActor;
    }

    public override string ToString()
    {
        return Actor + ".Transform";
    }
}
