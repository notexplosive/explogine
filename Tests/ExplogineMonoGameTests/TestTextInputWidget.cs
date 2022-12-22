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
            _emptyStringWidget.CursorIndex.Should().Be(_emptyStringWidget.Text.Length);
        }

        public class StartAtMiddle : HomeAndEnd
        {
            public StartAtMiddle()
            {
                _oneLineWidget.MoveTo(_oneLineWidget.Text.Length / 2);
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
                _oneLineWidget.MoveTo(_oneLineWidget.Text.Length);
            }
        }
    }
}
