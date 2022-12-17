using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;

namespace ExplogineMonoGame;

/// <summary>
///     This is its own interface for now so it's easier to update the signature. Once this feels more locked in we can
///     push it back down into ICartridge and the like.
/// </summary>
public interface IUpdateInput
{
    void UpdateInput(InputFrameState input, HitTestStack hitTestStack);
}
