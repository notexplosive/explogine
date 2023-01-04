using System;
using System.Collections.Generic;

namespace ExplogineMonoGame.Data;

public class DeferredActions
{
    private readonly List<Action> _list = new();

    public void Add(Action action)
    {
        _list.Add(action);
    }

    public void RunAllAndClear()
    {
        foreach (var item in _list)
        {
            item.Invoke();
        }
        
        _list.Clear();
    }
}
