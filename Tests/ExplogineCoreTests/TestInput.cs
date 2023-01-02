using System;
using ApprovalTests;
using ExplogineMonoGame;
using ExplogineMonoGame.Input;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Xunit;

namespace ExplogineCoreTests;

public class TestInput
{
    [Fact]
    public void nothing_pressed_by_default()
    {
        var input = new InputFrameState(InputSnapshot.Empty, InputSnapshot.Empty);
        input.Keyboard.IsAnyKeyDown().Should().BeFalse();
        input.Mouse.IsAnyButtonDown().Should().BeFalse();
        input.GamePad.IsAnyButtonDownOnAnyGamePad().Should().BeFalse();
    }

    [Fact]
    public void button_pressed()
    {
        var input = new InputFrameState(
            new InputSnapshot(
                new[] {Keys.Space},
                Vector2.Zero,
                new[]
                {
                    ButtonState.Pressed,
                    ButtonState.Released,
                    ButtonState.Released
                },
                0,
                new TextEnteredBuffer(),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.A),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.B),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.X),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.Y),
                    new GamePadDPad())
            ),
            InputSnapshot.Empty
        );

        input.Keyboard.IsAnyKeyDown().Should().BeTrue();
        input.Mouse.IsAnyButtonDown().Should().BeTrue();
        input.GamePad.IsAnyButtonDown(PlayerIndex.One).Should().BeTrue();
        input.GamePad.IsAnyButtonDownOnAnyGamePad().Should().BeTrue();

        input.Keyboard.GetButton(Keys.Space).IsDown.Should().BeTrue();
        input.Mouse.GetButton(MouseButton.Left).IsDown.Should().BeTrue();
        input.GamePad.GetButton(Buttons.A, PlayerIndex.One).IsDown.Should().BeTrue();

        input.Keyboard.GetButton(Keys.Space).WasPressed.Should().BeTrue();
        input.Mouse.GetButton(MouseButton.Left).WasPressed.Should().BeTrue();
        input.GamePad.GetButton(Buttons.A, PlayerIndex.One).WasPressed.Should().BeTrue();
    }

    [Fact]
    public void button_released()
    {
        var input = new InputFrameState(
            InputSnapshot.Empty,
            new InputSnapshot(
                new[] {Keys.Space},
                Vector2.Zero,
                new[]
                {
                    ButtonState.Pressed,
                    ButtonState.Released,
                    ButtonState.Released
                },
                0,
                new TextEnteredBuffer(),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.A),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.B),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.X),
                    new GamePadDPad()),
                new GamePadState(new GamePadThumbSticks(), new GamePadTriggers(), new GamePadButtons(Buttons.Y),
                    new GamePadDPad())
            )
        );

        input.Keyboard.IsAnyKeyDown().Should().BeFalse();
        input.Mouse.IsAnyButtonDown().Should().BeFalse();
        input.GamePad.IsAnyButtonDownOnAnyGamePad().Should().BeFalse();

        input.Keyboard.GetButton(Keys.Space).IsDown.Should().BeFalse();
        input.Mouse.GetButton(MouseButton.Left).IsDown.Should().BeFalse();
        input.GamePad.GetButton(Buttons.A, PlayerIndex.One).IsDown.Should().BeFalse();
        input.GamePad.GetButton(Buttons.B, PlayerIndex.Two).IsDown.Should().BeFalse();
        input.GamePad.GetButton(Buttons.X, PlayerIndex.Three).IsDown.Should().BeFalse();
        input.GamePad.GetButton(Buttons.Y, PlayerIndex.Four).IsDown.Should().BeFalse();

        input.Keyboard.GetButton(Keys.Space).WasReleased.Should().BeTrue();
        input.Mouse.GetButton(MouseButton.Left).WasReleased.Should().BeTrue();
        input.GamePad.GetButton(Buttons.A, PlayerIndex.One).WasReleased.Should().BeTrue();
        input.GamePad.GetButton(Buttons.B, PlayerIndex.Two).WasReleased.Should().BeTrue();
        input.GamePad.GetButton(Buttons.X, PlayerIndex.Three).WasReleased.Should().BeTrue();
        input.GamePad.GetButton(Buttons.Y, PlayerIndex.Four).WasReleased.Should().BeTrue();
    }

    [Fact]
    public void serialize_snapshot()
    {
        var before = new InputSnapshot(
            new[] {Keys.Space, Keys.A},
            Vector2.Zero,
            new[]
            {
                ButtonState.Pressed, ButtonState.Released, ButtonState.Released
            },
            0,
            new TextEnteredBuffer(new[] {'a', 'b', 'c'}),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.A),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.B),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.X),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.Y),
                new GamePadDPad())
        );

        var bytes = InputSerialization.AsString(before);
        var after = new InputSnapshot(bytes);

        after.MouseButtonStates.Should().BeEquivalentTo(before.MouseButtonStates);
        after.PressedKeys.Should().BeEquivalentTo(before.PressedKeys);
        after.MousePosition.Should().Be(before.MousePosition);

        foreach (var playerIndex in Enum.GetValues<PlayerIndex>())
        {
            after.GamePadSnapshotOfPlayer(playerIndex).PressedButtons.Should()
                .BeEquivalentTo(before.GamePadSnapshotOfPlayer(playerIndex).PressedButtons);
            after.GamePadSnapshotOfPlayer(playerIndex).LeftThumbstick.Should()
                .Be(before.GamePadSnapshotOfPlayer(playerIndex).LeftThumbstick);
            after.GamePadSnapshotOfPlayer(playerIndex).GamePadLeftTrigger.Should()
                .Be(before.GamePadSnapshotOfPlayer(playerIndex).GamePadLeftTrigger);
            after.GamePadSnapshotOfPlayer(playerIndex).GamePadRightTrigger.Should()
                .Be(before.GamePadSnapshotOfPlayer(playerIndex).GamePadRightTrigger);
        }
    }

    [Fact]
    public void serialization_output()
    {
        var snapshot = new InputSnapshot(
            new[] {Keys.Space, Keys.A},
            new Vector2(2, 4),
            new[] {ButtonState.Pressed, ButtonState.Released, ButtonState.Released},
            16,
            new TextEnteredBuffer(new[] {'a', 'b', 'c'}),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(0, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.A),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.B),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(0, 0.5f),
                new GamePadButtons(Buttons.X),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.Y),
                new GamePadDPad())
        );

        Approvals.Verify(snapshot);
    }

    [Fact]
    public void serialize_to_and_back()
    {
        var snapshot = new InputSnapshot(
            new[] {Keys.Space, Keys.A},
            new Vector2(2, 4),
            new[]
            {
                ButtonState.Pressed, ButtonState.Released, ButtonState.Released
            },
            16,
            new TextEnteredBuffer(new[] {'a', 'b', 'c'}),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.A),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.B),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.X),
                new GamePadDPad()),
            new GamePadState(
                new GamePadThumbSticks(new Vector2(1, 0), new Vector2(0, 1)),
                new GamePadTriggers(1, 0.5f),
                new GamePadButtons(Buttons.Y),
                new GamePadDPad())
        );

        var serialized = snapshot.Serialize();
        var deserialized = new InputSnapshot(serialized);
        var serializedBack = deserialized.Serialize();

        serialized.Should().Be(serializedBack);
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
