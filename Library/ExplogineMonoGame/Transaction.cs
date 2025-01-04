using System;

namespace ExplogineMonoGame;

public class Transaction
{
    private readonly Func<Action> _doAndGenerate;
    private Func<bool>? _shouldSkipFunction;
    private Action? _undo;

    public Transaction(string name, Func<Action> doAndGenerate)
    {
        _doAndGenerate = doAndGenerate;
        Name = name;
    }

    public string Name { get; }

    public bool ShouldSkip => _shouldSkipFunction != null && _shouldSkipFunction();

    public void Do()
    {
        _undo = _doAndGenerate();
    }

    public void Undo()
    {
        if (_undo == null)
        {
            Client.Debug.LogWarning("Dynamic action attempted to undo when it hasn't been Done yet");
        }

        _undo?.Invoke();
    }

    public void ShouldSkipWhen(Func<bool> when)
    {
        _shouldSkipFunction = when;
    }
}
