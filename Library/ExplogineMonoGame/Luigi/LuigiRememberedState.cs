using System.Collections.Generic;

namespace ExplogineMonoGame.Luigi;

public class LuigiRememberedState
{
    private readonly Dictionary<string, float> _rememberedScrollPositions = new();

    public void Forget()
    {
        _rememberedScrollPositions.Clear();
    }
    
    public float GetScrollPosition(string scrollPositionId)
    {
        return _rememberedScrollPositions.GetValueOrDefault(scrollPositionId, 0);
    }

    public void SetScrollPosition(string scrollPositionId, float panelScrollPositionY)
    {
        _rememberedScrollPositions[scrollPositionId] = panelScrollPositionY;
    }
}
