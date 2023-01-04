using System;
using System.Collections;
using System.Collections.Generic;
using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Rails;

public class Rail : IUpdateInputHook, IUpdateHook, IDrawHook, IEarlyDrawHook
{
    private readonly List<IHook> _hooks = new();

    public IHook this[Index i]
    {
        get => _hooks[i];
        set => _hooks[i] = value;
    }

    public int Count => _hooks.Count;

    public void Draw(Painter painter)
    {
        foreach (var hook in _hooks)
        {
            if (hook is IDrawHook drawHook)
            {
                drawHook.Draw(painter);
            }
        }
    }

    public void EarlyDraw(Painter painter)
    {
        foreach (var hook in _hooks)
        {
            if (hook is IEarlyDrawHook updateInputHook)
            {
                updateInputHook.EarlyDraw(painter);
            }
        }
    }

    public void Update(float dt)
    {
        foreach (var hook in _hooks)
        {
            if (hook is IUpdateHook updateHook)
            {
                updateHook.Update(dt);
            }
        }
    }

    public void UpdateInput(ConsumableInput input, HitTestStack hitTestStack)
    {
        foreach (var hook in _hooks)
        {
            if (hook is IUpdateInputHook updateInputHook)
            {
                updateInputHook.UpdateInput(input, hitTestStack);
            }
        }
    }

    public void Add(IHook hook)
    {
        if (!_hooks.Contains(hook))
        {
            _hooks.Add(hook);
        }
    }

    public void RemoveHook(IHook hook)
    {
        _hooks.Remove(hook);
    }

    public void Clear()
    {
        _hooks.Clear();
    }

    public IEnumerable<T> GetMatching<T>()
    {
        foreach (var item in _hooks)
        {
            if (item is T casted)
            {
                yield return casted;
            }
        }
    }
}