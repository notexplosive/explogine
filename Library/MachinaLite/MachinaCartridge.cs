using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;
using ExplogineMonoGame.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MachinaLite;

public abstract class MachinaCartridge : BasicGameCartridge
{
    private Vector2 _previousMouseWorldPosition;

    protected MachinaCartridge(IRuntime runtime) : base(runtime)
    {
    }

    protected List<Scene> Scenes { get; } = new();

    protected Scene AddSceneAsLayer()
    {
        var scene = new Scene(Runtime);
        Scenes.Add(scene);
        return scene;
    }

    protected void RemoveScene(Scene scene)
    {
        Scenes.Remove(scene);
    }

    public virtual void BeforeUpdate(float dt)
    {
    }

    public virtual void AfterUpdate(float dt)
    {
    }

    public override void Update(float dt)
    {
        BeforeUpdate(dt);
        var hitTestRoot = new HitTestRoot();

        for (var i = Scenes.Count - 1; i >= 0; i--)
        {
            var scene = Scenes[i];
            // Mouse
            var mouse = Client.Input.Mouse;
            var rawMousePosition = mouse.Position(Runtime.Window.ScreenToCanvas);
            if (mouse.WasAnyButtonPressedOrReleased())
            {
                foreach (var (state, button) in mouse.EachButton())
                {
                    if (mouse.GetButton(button).WasPressed || mouse.GetButton(button).WasReleased)
                    {
                        scene.OnMouseButton(button, rawMousePosition,
                            state.WasPressed ? ButtonState.Pressed : ButtonState.Released, hitTestRoot.BaseStack);
                    }
                }
            }

            scene.OnMouseUpdate(rawMousePosition,
                _previousMouseWorldPosition - scene.Camera.ScreenToWorld(rawMousePosition),
                mouse.Delta(Matrix.Identity), hitTestRoot.BaseStack);
            _previousMouseWorldPosition = mouse.Position(scene.Camera.ScreenToWorldMatrix);

            hitTestRoot.Resolve(scene.Camera.ScreenToWorld(rawMousePosition));
        }

        foreach (var scene in Scenes)
        {
            var keyboard = Client.Input.Keyboard;
            // Keyboard
            foreach (var (buttonFrameState, key) in keyboard.EachKey())
            {
                if (buttonFrameState.WasPressed)
                {
                    scene.OnKey(key, ButtonState.Pressed, keyboard.Modifiers);
                }

                if (buttonFrameState.WasReleased)
                {
                    scene.OnKey(key, ButtonState.Released, keyboard.Modifiers);
                }
            }

            // World Update
            scene.Update(dt);

            // kinda lame that we have to do this in the cartridge and it isn't just "handled for us"
            scene.FlushBuffers();
        }

        AfterUpdate(dt);
    }

    public override void Draw(Painter painter)
    {
        foreach (var scene in Scenes)
        {
            scene.PreDraw(painter);
            scene.Draw(painter);

            if (Client.Debug.IsActive)
            {
                scene.DebugDraw(painter);
            }
        }
    }
}
