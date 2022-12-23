using System;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Xunit;

namespace ExplogineMonoGameTests;

public class TestTextInputWidget
{
    [Fact]
    public void move_remove_and_replace()
    {
        var inputWidget = new TextInputWidget(Vector2.Zero, new Point(300, 300), new TestFont(), Depth.Middle,
            "Simple test text");
        inputWidget.MoveRight();
        inputWidget.MoveRight();
        inputWidget.ReverseBackspace();
        inputWidget.EnterCharacter('p');
        // ReSharper disable once StringLiteralTypo
        inputWidget.Text.Should().Be("Sipple test text");
    }

    public abstract class KeyboardNavigation
    {
        private readonly TextInputWidget _emptyStringWidget = new(Vector2.Zero, new Point(1000, 300), new TestFont(),
            Depth.Middle,
            "");

        private readonly TextInputWidget _manualManyLine = new(Vector2.Zero, new Point(1000, 300), new TestFont(),
            Depth.Middle,
            "Several\nLines\nOf\nText");

        private readonly TextInputWidget _naturalMultiLine = new(Vector2.Zero, new Point(300, 300), new TestFont(),
            Depth.Middle,
            "This should have natural linebreaks");

        private readonly TextInputWidget _oneLineWidget = new(Vector2.Zero, new Point(1000, 300), new TestFont(),
            Depth.Middle,
            "Simple test text");

        [Fact]
        public void one_line_home()
        {
            _oneLineWidget.MoveToStartOfLine();
            _oneLineWidget.CursorIndex.Should().Be(0);
        }

        [Fact]
        public void one_line_end()
        {
            _oneLineWidget.MoveToEndOfLine();
            _oneLineWidget.CursorIndex.Should().Be(_oneLineWidget.Text.Length);
        }

        [Fact]
        public void empty_home()
        {
            _emptyStringWidget.MoveToStartOfLine();
            _emptyStringWidget.CursorIndex.Should().Be(0);
        }

        [Fact]
        public void empty_end()
        {
            _emptyStringWidget.MoveToEndOfLine();
            _emptyStringWidget.CursorIndex.Should().Be(_emptyStringWidget.LastIndex);
        }

        [Fact]
        public void manual_multiline_home()
        {
            var startingLine = _manualManyLine.CurrentLine();
            _manualManyLine.MoveToStartOfLine();
            _manualManyLine.CurrentLine().Should().Be(startingLine);

            if (_manualManyLine.CursorIndex != 0)
            {
                _manualManyLine.MoveLeft();
                _manualManyLine.CurrentLine().Should().NotBe(startingLine);
            }
        }

        [Fact]
        public void manual_multiline_end()
        {
            var startingLine = _manualManyLine.CurrentLine();
            _manualManyLine.MoveToEndOfLine();
            _manualManyLine.CurrentLine().Should().Be(startingLine);

            if (_manualManyLine.CursorIndex != _manualManyLine.LastIndex)
            {
                _manualManyLine.MoveRight();
                _manualManyLine.CurrentLine().Should().NotBe(startingLine);
            }
        }

        [Fact]
        public void natural_multiline_home()
        {
            var startingLine = _naturalMultiLine.CurrentLine();
            _naturalMultiLine.MoveToStartOfLine();
            _naturalMultiLine.CurrentLine().Should().Be(startingLine);

            if (_naturalMultiLine.CursorIndex != 0)
            {
                _naturalMultiLine.MoveLeft();
                _naturalMultiLine.CurrentLine().Should().NotBe(startingLine);
            }
        }

        [Fact]
        public void natural_multiline_end()
        {
            var startingLine = _naturalMultiLine.CurrentLine();
            _naturalMultiLine.MoveToEndOfLine();
            _naturalMultiLine.CurrentLine().Should().Be(startingLine);

            if (_naturalMultiLine.CursorIndex != _naturalMultiLine.LastIndex)
            {
                _naturalMultiLine.MoveRight();
                _naturalMultiLine.CurrentLine().Should().NotBe(startingLine);
            }
        }

        [Fact]
        public void one_line_up_arrow()
        {
            var startingIndex = _oneLineWidget.CursorIndex;
            _oneLineWidget.MoveUp();
            _oneLineWidget.CursorIndex.Should().Be(startingIndex);
        }

        [Fact]
        public void one_line_down_arrow()
        {
            var startingIndex = _oneLineWidget.CursorIndex;
            _oneLineWidget.MoveDown();
            _oneLineWidget.CursorIndex.Should().Be(startingIndex);
        }

        [Fact]
        public void empty_up_arrow()
        {
            _emptyStringWidget.MoveUp();
            _emptyStringWidget.CursorIndex.Should().Be(0);
        }

        [Fact]
        public void empty_down_arrow()
        {
            _emptyStringWidget.MoveDown();
            _emptyStringWidget.CursorIndex.Should().Be(0);
        }

        [Fact]
        public void manual_multiline_up_arrow()
        {
            var column = _manualManyLine.CurrentColumn;
            _manualManyLine.MoveUp();
            var newLineLength = _manualManyLine.LineLength(_manualManyLine.CurrentLine());
            _manualManyLine.CurrentColumn.Should().Be(Math.Min(column, newLineLength - 1));
        }

        [Fact]
        public void manual_multiline_down_arrow()
        {
            // this should look like the up arrow equivalent but I'm lazy
            var column = _manualManyLine.CurrentColumn;
            _manualManyLine.MoveDown();
            _manualManyLine.CurrentColumn.Should().Be(column);
        }

        [Fact]
        public void natural_multiline_up_arrow()
        {
            var column = _naturalMultiLine.CurrentColumn;
            _naturalMultiLine.MoveUp();
            var newLineLength = _naturalMultiLine.LineLength(_naturalMultiLine.CurrentLine());
            _naturalMultiLine.CurrentColumn.Should().Be(Math.Min(column, newLineLength - 1));
        }

        [Fact]
        public void natural_multiline_down_arrow()
        {
            // this should look like the up arrow equivalent but I'm lazy
            var column = _naturalMultiLine.CurrentColumn;
            _naturalMultiLine.MoveDown();
            _naturalMultiLine.CurrentColumn.Should().Be(column);
        }

        public class StartAtMiddle : KeyboardNavigation
        {
            public StartAtMiddle()
            {
                _oneLineWidget.MoveTo(_oneLineWidget.LastIndex / 2);
                _manualManyLine.MoveTo(_manualManyLine.LastIndex / 2);
                _naturalMultiLine.MoveTo(_naturalMultiLine.LastIndex / 2);
            }
        }

        public class StartAtStart : KeyboardNavigation
        {
            public StartAtStart()
            {
                _oneLineWidget.MoveTo(0);
            }
        }

        public class StartAtEnd : KeyboardNavigation
        {
            public StartAtEnd()
            {
                _oneLineWidget.MoveTo(_oneLineWidget.LastIndex);
                _manualManyLine.MoveTo(_manualManyLine.LastIndex);
                _naturalMultiLine.MoveTo(_naturalMultiLine.LastIndex);
            }
        }
    }
}
