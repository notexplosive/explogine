using FluentAssertions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NotCore.Input;
using Xunit;

namespace NotCoreTests;

public class TestInput
{
    [Fact]
    public void nothing_pressed_by_default()
    {
        var input = new InputFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
        input.Keyboard.IsAnyKeyDown().Should().BeFalse();
        input.Mouse.IsAnyButtonDown().Should().BeFalse();
        input.GamePad.IsAnyButtonDown().Should().BeFalse();
    }

    [Fact]
    public void button_pressed()
    {
        var input = new InputFrameState(
            new InputSnapshot(
                new KeyboardState(Keys.Space),
                new MouseState(0, 0, 0, ButtonState.Pressed, ButtonState.Released, ButtonState.Released,
                    ButtonState.Released, ButtonState.Released),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.A),
                    new GamePadDPad())
            ),
            InputSnapshot.Empty
        );

        input.Keyboard.IsAnyKeyDown().Should().BeTrue();
        input.Mouse.IsAnyButtonDown().Should().BeTrue();
        input.GamePad.IsAnyButtonDown().Should().BeTrue();

        input.Keyboard.IsKeyDown(Keys.Space).Should().BeTrue();
        input.Mouse.IsButtonDown(MouseButton.Left).Should().BeTrue();
        input.GamePad.IsButtonDown(GamePadButton.A).Should().BeTrue();

        input.Keyboard.WasKeyPressed(Keys.Space).Should().BeTrue();
        input.Mouse.WasButtonPressed(MouseButton.Left).Should().BeTrue();
        input.GamePad.WasButtonPressed(GamePadButton.A).Should().BeTrue();
    }

    [Fact]
    public void button_released()
    {
        var input = new InputFrameState(
            InputSnapshot.Empty,
            new InputSnapshot(
                new KeyboardState(Keys.Space),
                new MouseState(0, 0, 0, ButtonState.Pressed, ButtonState.Released, ButtonState.Released,
                    ButtonState.Released, ButtonState.Released),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.A),
                    new GamePadDPad())
            )
        );

        input.Keyboard.IsAnyKeyDown().Should().BeFalse();
        input.Mouse.IsAnyButtonDown().Should().BeFalse();
        input.GamePad.IsAnyButtonDown().Should().BeFalse();

        input.Keyboard.IsKeyDown(Keys.Space).Should().BeFalse();
        input.Mouse.IsButtonDown(MouseButton.Left).Should().BeFalse();
        input.GamePad.IsButtonDown(GamePadButton.A).Should().BeFalse();

        input.Keyboard.WasKeyReleased(Keys.Space).Should().BeTrue();
        input.Mouse.WasButtonReleased(MouseButton.Left).Should().BeTrue();
        input.GamePad.WasButtonReleased(GamePadButton.A).Should().BeTrue();
    }

    [Fact]
    public void serialize_snapshot()
    {
        var before = new InputSnapshot(
            new KeyboardState(Keys.Space, Keys.A),
            new MouseState(
                0, 0, 0,
                ButtonState.Pressed, ButtonState.Released, ButtonState.Released,
                ButtonState.Released, ButtonState.Released),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.A),
                new GamePadDPad())
        );

        var bytes = InputSerialization.AsString(before);
        var after = new InputSnapshot(bytes);
        
        after.MouseButtonStates.Should().BeEquivalentTo(before.MouseButtonStates);
        after.PressedKeys.Should().BeEquivalentTo(before.PressedKeys);
        after.GamePadButtonStates.Should().BeEquivalentTo(before.GamePadButtonStates);
        after.LeftThumbstick.Should().Be(before.LeftThumbstick);
        after.MousePosition.Should().Be(before.MousePosition);
        after.GamePadLeftTrigger.Should().Be(before.GamePadLeftTrigger);
        after.GamePadRightTrigger.Should().Be(before.GamePadRightTrigger);
    }

    [Fact]
    public void serialization_sanity_check()
    {
        var buttonStates = InputSerialization.IntToStates(1, 5);

        buttonStates.Length.Should().Be(5);
        buttonStates[0].Should().Be(ButtonState.Pressed);
        buttonStates[1].Should().Be(ButtonState.Released);
    }
}
