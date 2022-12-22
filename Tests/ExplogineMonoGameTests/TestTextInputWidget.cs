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

    public abstract class HomeAndEnd
    {
        private readonly TextInputWidget _emptyStringWidget = new(Vector2.Zero, new Point(1000, 300), new TestFont(),
            Depth.Middle,
            "");
        
        private readonly TextInputWidget _oneLineWidget = new(Vector2.Zero, new Point(1000, 300), new TestFont(),
            Depth.Middle,
            "Simple test text");
        
        private readonly TextInputWidget _manualManyLine = new(Vector2.Zero, new Point(1000, 300), new TestFont(),
            Depth.Middle,
            "Several\nLines\nOf\nText");

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

        public class StartAtMiddle : HomeAndEnd
        {
            public StartAtMiddle()
            {
                _oneLineWidget.MoveTo(_oneLineWidget.LastIndex / 2);
                _manualManyLine.MoveTo(_manualManyLine.LastIndex / 2);
            }
        }
        
        public class StartAtStart : HomeAndEnd
        {
            public StartAtStart()
            {
                _oneLineWidget.MoveTo(0);
            }
        }
        
        public class StartAtEnd : HomeAndEnd
        {
            public StartAtEnd()
            {
                _oneLineWidget.MoveTo(_oneLineWidget.LastIndex);
                _manualManyLine.MoveTo(_manualManyLine.LastIndex);
            }
        }
    }
}
