using System;

namespace ExplogineMonoGame;

public class Transaction
{
    private readonly Func<Action> _doAndGenerate;
    private Action? _undo;

    public Transaction(string name, bool isSilent, Func<Action> doAndGenerate)
    {
        _doAndGenerate = doAndGenerate;
        Name = name;
        IsSilent = isSilent;
    }

    public string Name { get; }

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

    public bool IsSilent { get; }
}
