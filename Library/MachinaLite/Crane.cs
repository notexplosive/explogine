using System.Diagnostics;
using ExplogineMonoGame;
using ExplogineMonoGame.HitTesting;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MachinaLite;

public interface ICrane
{
    /// <summary>
    ///     Called every frame with the delta time since last frame.
    /// </summary>
    /// <param name="dt">Delta time since last frame</param>
    public void Update(float dt);

    /// <summary>
    ///     Runs after all updates have completed.
    /// </summary>
    public void OnPostUpdate();

    /// <summary>
    ///     Called just before Updates() are issued on any iterables that were just added.
    /// </summary>
    public void Start();

    /// <summary>
    ///     Called on every scene BEFORE any real drawing happens, does not imply spriteBatch.Begin
    /// </summary>
    /// <param name="painter"></param>
    public void PreDraw(Painter painter);

    /// <summary>
    ///     Called every visual frame, spriteBatch.Begin has already been called.
    /// </summary>
    /// <param name="painter"></param>
    public void Draw(Painter painter);

    /// <summary>
    ///     Happens after Draw if the DebugLevel is higher than Passive
    /// </summary>
    /// <param name="spriteBatch"></param>
    public void DebugDraw(Painter painter);

    /// <summary>
    ///     Called every time the scroll wheel is incremented. Also subsequently calls MouseMove with zero delta.
    /// </summary>
    /// <param name="scrollDelta"></param>
    public void OnScroll(int scrollDelta);

    /// <summary>
    ///     Called when the object is removed from its iterable set
    /// </summary>
    public void OnDeleteFinished();

    /// <summary>
    ///     Called when the object is flagged for removal from its iterable set
    /// </summary>
    public void OnDeleteImmediate();

    /// <summary>
    ///     Called when user presses or releases a key
    /// </summary>
    /// <param name="key">Key pressed or released</param>
    /// <param name="state">Enum for if the button was pressed or released</param>
    /// <param name="modifiers">Modifiers that are currently pressed (ctrl, alt, shift)</param>
    public void OnKey(Keys key, ButtonState state, ModifierKeys modifiers);

    /// <summary>
    ///     Called every time mouse moves.
    /// </summary>
    /// <param name="currentPosition">Mouse position transformed into your context</param>
    /// <param name="worldDelta">Mouse movement transformed to your context</param>
    /// <param name="rawDelta">Mouse movement delta in real-world screen pixels</param>
    /// <param name="hitTestStack"></param>
    public void OnMouseUpdate(Vector2 currentPosition, Vector2 worldDelta, Vector2 rawDelta,
        HitTestStack hitTestStack);

    /// <summary>
    ///     Called when user presses or releases the mouse
    /// </summary>
    /// <param name="button">Mouse button pressed or released</param>
    /// <param name="currentPosition">Mouse position transformed to your context</param>
    /// <param name="state">Button state reflecting if the mouse was pressed or released</param>
    /// <param name="hitTestStack"></param>
    public void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state, HitTestStack hitTestStack);

    /// <summary>
    ///     TEST ONLY: Effectively this is a zero second update that doesn't call Update.
    ///     It will flush the newIterable and deletedIterable buffers.
    /// </summary>
    public void FlushBuffers();

    /// <summary>
    ///     When a TextInput event is fired
    /// </summary>
    /// <param name="textInputEventArgs"></param>
    void OnTextInput(TextInputEventArgs textInputEventArgs);
}

/// <summary>
///     Parent class for Scene and Actor that describes how each entry-point method iterates over the iterables and runs
///     the same entry-point method.
///     Why is it called a Crane? Couldn't tell you, sometimes you just need a name for things.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Crane<T> : ICrane where T : ICrane
{
    protected readonly List<T> Iterables = new();
    private readonly List<T> _iterablesCreatedThisFrame = new();
    private readonly List<T> _iterablesDeletedThisFrame = new();
    private readonly List<T> _iterablesGentlyRemovedThisFrame = new();

    public virtual void Update(float dt)
    {
        Functions.ForEach(Iterables, iterable => { iterable.Update(dt); });
        OnPostUpdate();
    }

    public void FlushBuffers()
    {
        FlushCreatedIterables();
        Functions.ForEach(Iterables, iterable => { iterable.FlushBuffers(); });
        FlushRemovedAndDeletedIterables();
    }

    public virtual void OnPostUpdate()
    {
        Functions.ForEach(Iterables, iterable => { iterable.OnPostUpdate(); });
    }

    public virtual void Start()
    {
        Functions.ForEach(Iterables, iterable => { iterable.Start(); });
    }

    public virtual void PreDraw(Painter painter)
    {
        Functions.ForEach(Iterables, iterable => { iterable.PreDraw(painter); });
    }

    public virtual void Draw(Painter painter)
    {
        Functions.ForEach(Iterables, iterable => { iterable.Draw(painter); });
    }

    public virtual void DebugDraw(Painter painter)
    {
        Functions.ForEach(Iterables, iterable => { iterable.DebugDraw(painter); });
    }

    public virtual void OnScroll(int scrollDelta)
    {
        Functions.ForEach(Iterables, iterable => { iterable.OnScroll(scrollDelta); });
    }

    public virtual void OnDeleteFinished()
    {
        Functions.ForEach(Iterables, iterable => { iterable.OnDeleteFinished(); });
    }

    public virtual void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
    {
        Functions.ForEach(Iterables, iterable => { iterable.OnKey(key, state, modifiers); });
    }

    public virtual void OnTextInput(TextInputEventArgs textInputEventArgs)
    {
        Functions.ForEach(Iterables, iterable => { iterable.OnTextInput(textInputEventArgs); });
    }

    public virtual void OnMouseUpdate(Vector2 currentPosition, Vector2 worldDelta, Vector2 rawDelta,
        HitTestStack hitTestStack)
    {
        Functions.ForEach(Iterables,
            iterable => { iterable.OnMouseUpdate(currentPosition, worldDelta, rawDelta, hitTestStack); });
    }

    public virtual void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state,
        HitTestStack hitTestStack)
    {
        Functions.ForEach(Iterables,
            iterable => { iterable.OnMouseButton(button, currentPosition, state, hitTestStack); });
    }

    public void OnDeleteImmediate()
    {
        Functions.ForEach(Iterables, iterable => { iterable.OnDeleteImmediate(); });
    }

    protected void AddIterable(T newIterable)
    {
        _iterablesCreatedThisFrame.Add(newIterable);
    }

    /// <summary>
    ///     Is a child iterable pending deletion?
    /// </summary>
    /// <param name="iterable"></param>
    /// <returns></returns>
    public bool IsIterablePendingDeletion(T? iterable)
    {
        if (iterable == null)
        {
            return false;
        }
        
        return _iterablesDeletedThisFrame.Contains(iterable) ||
               _iterablesGentlyRemovedThisFrame.Contains(iterable);
    }

    /// <summary>
    ///     Remove this iterable, assume it's being deleted (ie: dispose any resources)
    /// </summary>
    /// <param name="removedIterable"></param>
    protected void DeleteIterable(T removedIterable)
    {
        _iterablesDeletedThisFrame.Add(removedIterable);
        removedIterable.OnDeleteImmediate();
    }

    /// <summary>
    ///     Remove this iterable, but don't assume it's being deleted
    /// </summary>
    protected void GentlyRemoveIterable(T removedIterable)
    {
        _iterablesGentlyRemovedThisFrame.Add(removedIterable);
    }

    private void FlushCreatedIterables()
    {
        while (_iterablesCreatedThisFrame.Count > 0)
        {
            var iterable = _iterablesCreatedThisFrame[0];
            _iterablesCreatedThisFrame.RemoveAt(0);

            Iterables.Add(iterable);
            iterable.Start();
        }

        Debug.Assert(_iterablesCreatedThisFrame.Count == 0);
    }

    private void FlushRemovedAndDeletedIterables()
    {
        while (_iterablesDeletedThisFrame.Count > 0)
        {
            var iterable = _iterablesDeletedThisFrame[0];
            _iterablesDeletedThisFrame.RemoveAt(0);

            if (Iterables.Remove(iterable))
            {
                iterable.OnDeleteFinished();
            }
        }

        Debug.Assert(_iterablesDeletedThisFrame.Count == 0);

        while (_iterablesGentlyRemovedThisFrame.Count > 0)
        {
            var iterable = _iterablesGentlyRemovedThisFrame[0];
            _iterablesGentlyRemovedThisFrame.RemoveAt(0);
            Iterables.Remove(iterable);
        }

        Debug.Assert(_iterablesGentlyRemovedThisFrame.Count == 0);
    }
}

public static class Functions
{
    public static void ForEach<T>(List<T> iterables, Action<T> action) where T : ICrane
    {
        foreach (var item in new List<T>(iterables))
        {
            action(item);
        }
    }
}

public abstract class NonIteratingCrane : ICrane
{
    public virtual void Start()
    {
    }

    public virtual void DebugDraw(Painter painter)
    {
    }

    public virtual void Draw(Painter painter)
    {
    }

    public virtual void OnKey(Keys key, ButtonState state, ModifierKeys modifiers)
    {
    }

    public virtual void OnMouseButton(MouseButton button, Vector2 currentPosition, ButtonState state,
        HitTestStack hitTestStack)
    {
    }

    public virtual void OnMouseUpdate(Vector2 currentPosition, Vector2 worldDelta, Vector2 rawDelta,
        HitTestStack hitTestStack)
    {
    }

    public virtual void OnDeleteFinished()
    {
    }

    public virtual void OnScroll(int scrollDelta)
    {
    }

    public virtual void PreDraw(Painter painter)
    {
    }

    public virtual void Update(float dt)
    {
    }

    public virtual void OnPostUpdate()
    {
    }

    public virtual void OnTextInput(TextInputEventArgs inputEventArgs)
    {
    }

    public virtual void OnDeleteImmediate()
    {
    }

    public void FlushBuffers()
    {
    }
}
