using System;
using System.Collections.Generic;

namespace ExplogineMonoGame.Data;

public class DeferredActions
{
    private readonly List<Action> _list = new();
    private bool _isRunning;

    public void Add(Action action)
    {
        if (_isRunning)
        {
            action();
        }
        else
        {
            _list.Add(action);
        }
    }

    public void RunAllAndClear()
    {
        _isRunning = true;
        foreach (var item in _list)
        {
            item.Invoke();
        }

        _isRunning = false;
        _list.Clear();
    }
}
