using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame.Debugging;

public class FrameStep
{
    public void Update(float dt)
    {
        if (Client.Debug.IsPassive)
        {
            if (Client.CartridgeChain.IsFrozen)
            {
                if (Client.Input.Mouse.ScrollDelta() < 0)
                {
                    Client.CartridgeChain.UpdateCurrentCartridge(dt);
                }
            }

            if (Client.Input.Keyboard.GetButton(Keys.Space).WasPressed && Client.Input.Keyboard.Modifiers.Control)
            {
                Client.CartridgeChain.IsFrozen = !Client.CartridgeChain.IsFrozen;
                var enabledString = Client.CartridgeChain.IsFrozen ? "Enabled" : "Disabled";
                Client.Debug.Log($"FrameStep {enabledString}");
            }
        }
    }
}
