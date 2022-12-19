using ApprovalTests;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Layout;
using FluentAssertions;
using Microsoft.Xna.Framework;
using Xunit;

namespace ExplogineMonoGameTests;

public class TestLayout
{
    [Fact]
    public void build_serial_from_real_layout()
    {
        var layout = L.Compute(
            new RectangleF(0, 0, 500, 500),
            new LayoutElementGroup(
                new ArrangementSettings(
                    Orientation.Vertical,
                    Margin: new Vector2(25, 25)),
                new[]
                {
                    L.Group(L.FillHorizontal("title-bar", 40),
                        new ArrangementSettings(
                            Alignment: Alignment.Center),
                        new[]
                        {
                            L.FixedElement("icon", 32, 32),
                            L.Group(L.FillHorizontal(32),
                                new ArrangementSettings(
                                    Alignment: Alignment.CenterRight,
                                    PaddingBetweenElements: 3),
                                new[]
                                {
                                    L.FillHorizontal("title", 32),
                                    L.FixedElement("minimize-button", 32, 32),
                                    L.FixedElement("fullscreen-button", 32, 32),
                                    L.FixedElement("close-button", 32, 32)
                                })
                        }),
                    L.FillBoth("body")
                }));

        Approvals.Verify(L.ToJson(layout));
    }

    [Fact]
    public void serialize_and_back()
    {
        var layout = L.Compute(
            new RectangleF(0, 0, 500, 500),
            new LayoutElementGroup(
                new ArrangementSettings(
                    Orientation.Vertical,
                    Margin: new Vector2(25, 25)),
                new[]
                {
                    L.Group(L.FillHorizontal("title-bar", 40),
                        new ArrangementSettings(
                            Alignment: Alignment.Center),
                        new[]
                        {
                            L.FixedElement("icon", 32, 32),
                            L.Group(L.FillHorizontal(32),
                                new ArrangementSettings(
                                    Alignment: Alignment.CenterRight,
                                    PaddingBetweenElements: 3),
                                new[]
                                {
                                    L.FillHorizontal("title", 32),
                                    L.FixedElement("minimize-button", 32, 32),
                                    L.FixedElement("fullscreen-button", 32, 32),
                                    L.FixedElement("close-button", 32, 32)
                                })
                        }),
                    L.FillBoth("body")
                }));

        var serialized = L.ToJson(layout);
        var deserialized = L.FromJson(new RectangleF(0, 0, 500, 500), serialized);

        L.ToJson(deserialized).Should().Be(serialized);

    }
}
