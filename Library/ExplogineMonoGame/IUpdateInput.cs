using System.Collections.Generic;
using System.Linq;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ExplogineMonoGame;

/// <summary>
///     This is its own interface for now so it's easier to update the signature. Once this feels more locked in we can
///     push it back down into ICartridge and the like.
/// </summary>
public interface IUpdateInput
{
    void UpdateInput(ConsumableInput input, HitTestStack hitTestStack);
}

public class ConsumableInput
{
    public ConsumableInput(InputFrameState raw)
    {
        Raw = raw;
        Keyboard = new ConsumableKeyboard(Raw.Keyboard);
        Mouse = new ConsumableMouse(Raw.Mouse);
    }

    public InputFrameState Raw { get; }
    public ConsumableKeyboard Keyboard { get; }
    public ConsumableMouse Mouse { get; }

    public class ConsumableMouse
    {
        private readonly HashSet<MouseButton> _consumedButtons = new();
        private readonly MouseFrameState _raw;

        public ConsumableMouse(MouseFrameState raw)
        {
            _raw = raw;
        }

        public Vector2 CanvasPosition(Matrix? position = null)
        {
            return _raw.CanvasPosition(position);
        }

        public ButtonFrameState GetButton(MouseButton button)
        {
            return _consumedButtons.Contains(button) ? new ButtonFrameState(false, false) : _raw.GetButton(button);
        }

        public Vector2 Position(Matrix? worldMatrix = null)
        {
            return _raw.Position(worldMatrix);
        }

        public int ScrollDelta()
        {
            return _raw.ScrollDelta();
        }

        public Vector2 Delta(Matrix? matrix = null)
        {
            return _raw.Delta(matrix);
        }

        public Vector2 CanvasDelta(Matrix? matrix = null)
        {
            return _raw.CanvasDelta(matrix);
        }

        public IEnumerable<(ButtonFrameState, MouseButton)> EachButton()
        {
            foreach (var key in _raw.EachButton())
            {
                if (_consumedButtons.Contains(key.Item2))
                {
                    yield return (ButtonFrameState.Empty, key.Item2);
                }
                else
                {
                    yield return key;
                }
            }
        }

        public bool WasAnyButtonPressedOrReleased()
        {
            return _raw.WasAnyButtonPressedOrReleased();
        }

        public void Consume(MouseButton button)
        {
            _consumedButtons.Add(button);
        }
    }

    public class ConsumableKeyboard
    {
        private readonly HashSet<Keys> _consumedKeys = new();
        private readonly HashSet<char> _consumedTextInput = new();
        private readonly KeyboardFrameState _raw;

        public ConsumableKeyboard(KeyboardFrameState raw)
        {
            _raw = raw;
        }

        public ModifierKeys Modifiers => _raw.Modifiers;

        public ButtonFrameState GetButton(Keys keys, bool consume = false)
        {
            var result = _consumedKeys.Contains(keys) ? ButtonFrameState.Empty : _raw.GetButton(keys);
            if (consume)
            {
                Consume(keys);
            }
            return result;
        }

        public char[] GetEnteredCharacters()
        {
            var result = new List<char>();

            foreach(var character in _raw.GetEnteredCharacters())
            {
                if (!_consumedTextInput.Contains(character))
                {
                    result.Add(character);
                }
            }

            return result.ToArray();
        }

        public IEnumerable<(ButtonFrameState, Keys)> EachKey()
        {
            foreach (var key in _raw.EachKey())
            {
                if (_consumedKeys.Contains(key.Item2))
                {
                    yield return (ButtonFrameState.Empty, key.Item2);
                }
                else
                {
                    yield return key;
                }
            }
        }

        public bool IsAnyKeyDown()
        {
            return _raw.IsAnyKeyDown();
        }

        public void Consume(Keys keys)
        {
            _consumedKeys.Add(keys);
        }

        public void ConsumeTextInput(char c)
        {
            _consumedTextInput.Add(c);
            
            var keys = InputUtil.CharToKeys(c);
            if (keys.HasValue && !_consumedKeys.Contains(keys.Value))
            {
                Consume(keys.Value);
            }
        }
    }
}
