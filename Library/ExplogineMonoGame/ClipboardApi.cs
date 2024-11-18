using System;
using TextCopy;

namespace ExplogineMonoGame;

public class ClipboardApi
{
    public bool IsAllowedOnPlatform()
    {
        try
        {
            ClipboardService.GetText();
            
            // if it didn't throw, we're good!
            return true;
        }
        catch (Exception exception)
        {
            Client.Debug.LogError(exception);
            return false;
        }
    }
    
    public string? Get()
    {
        if (!IsAllowedOnPlatform())
        {
            return null;
        }
        
        return ClipboardService.GetText();
    }

    public void Set(string text)
    {
        if (!IsAllowedOnPlatform())
        {
            return;
        }
        
        ClipboardService.SetText(text);
    }
}
