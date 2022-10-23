using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExplogineMonoGame;

public readonly record struct TextEnteredBuffer(char[] Characters)
{
    public TextEnteredBuffer WithAddedCharacter(char newCharacter)
    {
        // normally an array copy like this would be awful, but this only ever comes up if you press multiple keys in one frame
        var oldBuffer = Characters ?? Array.Empty<char>();

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
        if (Characters == null)
        {
            return string.Empty;
        }

        var intList = new List<int>();
        foreach(var character in Characters)
        {
            intList.Add(character);
        }
        
        return string.Join(",", intList);
    }
}
