using System;

namespace ExplogineMonoGame.AssetManagement;

public class MissingContentException : Exception
{
    public MissingContentException(string str) : base(str)
    {
        
    }
}
