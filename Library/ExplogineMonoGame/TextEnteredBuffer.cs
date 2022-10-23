using System;
using System.Collections.Generic;

namespace ExplogineMonoGame;

public readonly record struct TextEnteredBuffer(char[]? MaybeCharacters)
{
    public char[] Characters => MaybeCharacters ?? Array.Empty<char>();

    public TextEnteredBuffer WithAddedCharacter(char newCharacter)
    {
        // normally an array copy like this would be awful, but this only ever comes up if you press multiple keys in one frame
        var oldBuffer = Characters;

        var newBuffer = new char[oldBuffer.Length + 1];

        for (var i = 0; i < oldBuffer.Length; i++)
        {
            newBuffer[i] = oldBuffer[i];
        }

        newBuffer[oldBuffer.Length] = newCharacter;

        return new TextEnteredBuffer(newBuffer);
    }

    public override string ToString()
    {
        if (MaybeCharacters == null)
        {
            return string.Empty;
        }

        var intList = new List<int>();
        foreach (var character in MaybeCharacters)
        {
            intList.Add(character);
        }

        return string.Join(",", intList);
    }
}
