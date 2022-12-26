using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApprovalTests;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Xunit;

namespace ExplogineMonoGameTests;

public class TestTextInputWidget
{
    [Fact]
    public void move_by_words_pinning()
    {
        var str = "The fish was delish and it made quite a dish";
        var inputWidget = new TextInputWidget(Vector2.Zero, new Point(300, 300), new TestFont(), Depth.Middle, false,
            // ReSharper disable once StringLiteralTypo
            str);

        var resultLines = new List<string>();
        
        resultLines.Add(str);
        
        for (int i = 0; i < str.Length; i++)
        {
            var from = i;
            var toLeft = inputWidget.GetWordBoundaryLeftOf(i);
            var toRight = inputWidget.GetWordBoundaryRightOf(i);
            
            var chars = Enumerable.Repeat(' ', str.Length + 1).ToArray();

            for (int j = 0; j < str.Length + 1; j++)
            {
                if (j == from)
                {
                    chars[j] = '-';
                }

                if (j == toLeft)
                {
                    chars[j] = '<';
                }
                
                if (j == toRight)
                {
                    chars[j] = '>';
                }
            }
            
            resultLines.Add(string.Join("",chars));
        }

        Approvals.Verify(string.Join('\n', resultLines));
    }

    [Fact]
    public void move_remove_and_replace()
    {
        var inputWidget = new TextInputWidget(Vector2.Zero, new Point(300, 300), new TestFont(), Depth.Middle, false,
            "Simple test text");
        inputWidget.MoveRight(false);
        inputWidget.MoveRight(false);
        inputWidget.ReverseBackspace(false);
        inputWidget.EnterCharacter('p');
        // ReSharper disable once StringLiteralTypo
        inputWidget.Text.Should().Be("Sipple test text");
    }

    public abstract class KeyboardNavigation
    {
        private readonly TextInputWidget _emptyStringWidget = new(Vector2.Zero, new Point(1000, 300), new TestFont(),
            Depth.Middle, false,
            "");

        private readonly TextInputWidget _manualManyLine = new(Vector2.Zero, new Point(1000, 300), new TestFont(),
            Depth.Middle, false,
            "Several\nLines\nOf\nText");

        private readonly TextInputWidget _naturalMultiLine = new(Vector2.Zero, new Point(300, 300), new TestFont(),
            Depth.Middle, false,
            "This should have natural linebreaks");

        private readonly TextInputWidget _oneLineWidget = new(Vector2.Zero, new Point(1000, 300), new TestFont(),
            Depth.Middle, false,
            "Simple test text");

        [Fact]
        public void one_line_home()
        {
            _oneLineWidget.MoveToStartOfLine(false);
            _oneLineWidget.CursorIndex.Should().Be(0);
        }

        [Fact]
        public void one_line_end()
        {
            _oneLineWidget.MoveToEndOfLine(false);
            _oneLineWidget.CursorIndex.Should().Be(_oneLineWidget.Text.Length);
        }

        [Fact]
        public void empty_home()
        {
            _emptyStringWidget.MoveToStartOfLine(false);
            _emptyStringWidget.CursorIndex.Should().Be(0);
        }

        [Fact]
        public void empty_end()
        {
            _emptyStringWidget.MoveToEndOfLine(false);
            _emptyStringWidget.CursorIndex.Should().Be(_emptyStringWidget.LastIndex);
        }

        [Fact]
        public void manual_multiline_home()
        {
            var startingLine = _manualManyLine.CurrentLine;
            _manualManyLine.MoveToStartOfLine(false);
            _manualManyLine.CurrentLine.Should().Be(startingLine);

            if (_manualManyLine.CursorIndex != 0)
            {
                _manualManyLine.MoveLeft(false);
                _manualManyLine.CurrentLine.Should().NotBe(startingLine);
            }
        }

        [Fact]
        public void manual_multiline_end()
        {
            var startingLine = _manualManyLine.CurrentLine;
            _manualManyLine.MoveToEndOfLine(false);
            _manualManyLine.CurrentLine.Should().Be(startingLine);

            if (_manualManyLine.CursorIndex != _manualManyLine.LastIndex)
            {
                _manualManyLine.MoveRight(false);
                _manualManyLine.CurrentLine.Should().NotBe(startingLine);
            }
        }

        [Fact]
        public void natural_multiline_home()
        {
            var startingLine = _naturalMultiLine.CurrentLine;
            _naturalMultiLine.MoveToStartOfLine(false);
            _naturalMultiLine.CurrentLine.Should().Be(startingLine);

            if (_naturalMultiLine.CursorIndex != 0)
            {
                _naturalMultiLine.MoveLeft(false);
                _naturalMultiLine.CurrentLine.Should().NotBe(startingLine);
            }
        }

        [Fact]
        public void natural_multiline_end()
        {
            var startingLine = _naturalMultiLine.CurrentLine;
            _naturalMultiLine.MoveToEndOfLine(false);
            _naturalMultiLine.CurrentLine.Should().Be(startingLine);

            if (_naturalMultiLine.CursorIndex != _naturalMultiLine.LastIndex)
            {
                _naturalMultiLine.MoveRight(false);
                _naturalMultiLine.CurrentLine.Should().NotBe(startingLine);
            }
        }

        [Fact]
        public void one_line_up_arrow()
        {
            var startingIndex = _oneLineWidget.CursorIndex;
            _oneLineWidget.MoveUp(false);
            _oneLineWidget.CursorIndex.Should().Be(startingIndex);
        }

        [Fact]
        public void one_line_down_arrow()
        {
            var startingIndex = _oneLineWidget.CursorIndex;
            _oneLineWidget.MoveDown(false);
            _oneLineWidget.CursorIndex.Should().Be(startingIndex);
        }

        [Fact]
        public void empty_up_arrow()
        {
            _emptyStringWidget.MoveUp(false);
            _emptyStringWidget.CursorIndex.Should().Be(0);
        }

        [Fact]
        public void empty_down_arrow()
        {
            _emptyStringWidget.MoveDown(false);
            _emptyStringWidget.CursorIndex.Should().Be(0);
        }

        [Fact]
        public void manual_multiline_up_arrow()
        {
            var column = _manualManyLine.CurrentColumn;
            _manualManyLine.MoveUp(false);
            var newLineLength = _manualManyLine.LineLength(_manualManyLine.CurrentLine);
            _manualManyLine.CurrentColumn.Should().Be(Math.Min(column, newLineLength - 1));
        }

        [Fact]
        public void manual_multiline_down_arrow()
        {
            // this should look like the up arrow equivalent but I'm lazy
            var column = _manualManyLine.CurrentColumn;
            _manualManyLine.MoveDown(false);
            _manualManyLine.CurrentColumn.Should().Be(column);
        }

        [Fact]
        public void natural_multiline_up_arrow()
        {
            var column = _naturalMultiLine.CurrentColumn;
            _naturalMultiLine.MoveUp(false);
            var newLineLength = _naturalMultiLine.LineLength(_naturalMultiLine.CurrentLine);
            _naturalMultiLine.CurrentColumn.Should().Be(Math.Min(column, newLineLength - 1));
        }

        [Fact]
        public void natural_multiline_down_arrow()
        {
            // this should look like the up arrow equivalent but I'm lazy
            var column = _naturalMultiLine.CurrentColumn;
            _naturalMultiLine.MoveDown(false);
            _naturalMultiLine.CurrentColumn.Should().Be(column);
        }

        public class StartAtMiddle : KeyboardNavigation
        {
            public StartAtMiddle()
            {
                _oneLineWidget.MoveTo(_oneLineWidget.LastIndex / 2, false);
                _manualManyLine.MoveTo(_manualManyLine.LastIndex / 2, false);
                _naturalMultiLine.MoveTo(_naturalMultiLine.LastIndex / 2, false);
            }
        }

        public class StartAtStart : KeyboardNavigation
        {
            public StartAtStart()
            {
                _oneLineWidget.MoveTo(0, false);
            }
        }

        public class StartAtEnd : KeyboardNavigation
        {
            public StartAtEnd()
            {
                _oneLineWidget.MoveTo(_oneLineWidget.LastIndex, false);
                _manualManyLine.MoveTo(_manualManyLine.LastIndex, false);
                _naturalMultiLine.MoveTo(_naturalMultiLine.LastIndex, false);
            }
        }
    }
}
