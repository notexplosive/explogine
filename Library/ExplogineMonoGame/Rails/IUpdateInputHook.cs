using ExplogineMonoGame.Data;

namespace ExplogineMonoGame.Rails;

public interface IUpdateInputHook : IHook
{
    void UpdateInput(ConsumableInput input, HitTestStack hitTestStack);
}
