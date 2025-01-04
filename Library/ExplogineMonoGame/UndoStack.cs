using System;
using ExplogineCore.Data;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame;

public class UndoStack
{
    private readonly LimitedDeque<Transaction> _redoStack = new();
    private readonly LimitedDeque<Transaction> _undoStack = new(10000);
    public event Action<string>? RequestToast;

    public void Clear()
    {
        _undoStack.Clear();
        _redoStack.Clear();
    }

    private void PushTransaction(Transaction transaction)
    {
        _undoStack.Push(transaction);
        _redoStack.Clear();
    }

    /// <summary>
    ///     Awkward helper method.
    ///     Does an undo or a redo (depending on stack order and method).
    /// </summary>
    private UndoRedoResult UndoRedo(string? message, LimitedDeque<Transaction> popStack,
        LimitedDeque<Transaction> pushStack, Action<Transaction> method)
    {
        if (popStack.HasContent())
        {
            var transaction = popStack.PopUnsafe();
            method(transaction);
            pushStack.Push(transaction);

            // If the transaction is marked silent, and we have transactions before this one, do the next one back 
            if (transaction.ShouldSkip && popStack.HasContent())
            {
                // silent actions don't count as an "action"
                UndoRedo(message, popStack, pushStack, method);
                return UndoRedoResult.DidSilent;
            }

            if (!string.IsNullOrEmpty(message))
            {
                RequestToast?.Invoke($"{message} {transaction.Name}");
            }

            return UndoRedoResult.DidSomething;
        }

        return UndoRedoResult.Nothing;
    }

    private void Undo()
    {
        UndoRedo("Undo", _undoStack, _redoStack, transaction => transaction.Undo());
    }

    private void UndoMany(int count)
    {
        UndoRedoMany(count, "Undo", _undoStack, _redoStack, transaction => transaction.Undo());
    }

    private void Redo()
    {
        UndoRedo("Redo", _redoStack, _undoStack, transaction => transaction.Do());
    }

    private void RedoMany(int count)
    {
        UndoRedoMany(count, "Redo", _redoStack, _undoStack, transaction => transaction.Do());
    }

    private void UndoRedoMany(int count, string aggregateMessage, LimitedDeque<Transaction> popStack,
        LimitedDeque<Transaction> pushStack,
        Action<Transaction> method)
    {
        var successCount = 0;
        var didSilent = false;
        while (successCount < count)
        {
            var result = UndoRedo(null, popStack, pushStack, method);

            if (result == UndoRedoResult.DidSomething)
            {
                successCount++;
            }

            if (result == UndoRedoResult.DidSilent)
            {
                didSilent = true;
            }

            if (result == UndoRedoResult.Nothing)
            {
                break;
            }
        }

        if (successCount == 0 && didSilent)
        {
            // if all we did was a silent, report that as 1
            successCount = 1;
        }

        if (successCount > 0)
        {
            RequestToast?.Invoke($"{aggregateMessage} {successCount} actions");
        }
    }

    public void UpdateKeyboardInput(ConsumableInput input)
    {
        if (input.Keyboard.Modifiers.Control && input.Keyboard.GetButton(Keys.Z, true).WasPressed)
        {
            Undo();
        }

        if ((input.Keyboard.Modifiers.ControlShift && input.Keyboard.GetButton(Keys.Z, true).WasPressed) || (input.Keyboard.Modifiers.Control && input.Keyboard.GetButton(Keys.Y, true).WasPressed))
        {
            Redo();
        }

        var undoManyCount = 10;
        if (input.Keyboard.Modifiers.ControlAlt && input.Keyboard.GetButton(Keys.Z, true).WasPressed)
        {
            UndoMany(undoManyCount);
        }

        if ((input.Keyboard.Modifiers.ControlAltShift && input.Keyboard.GetButton(Keys.Z, true).WasPressed) ||
            (input.Keyboard.Modifiers.ControlAlt && input.Keyboard.GetButton(Keys.Y, true).WasPressed))
        {
            RedoMany(undoManyCount);
        }
    }

    public Transaction AddTransaction(string name, Func<Action> doAndGenerateUndo)
    {
        Client.Debug.LogVerbose($"Added Undo Transaction: {name}");
        var transaction = new Transaction(name, doAndGenerateUndo);
        transaction.Do();
        PushTransaction(transaction);
        return transaction;
    }

    private enum UndoRedoResult
    {
        Nothing,
        DidSomething,
        DidSilent
    }
}
