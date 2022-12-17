using ApprovalTests;
using ExplogineCore.Data;
using ExplogineMonoGame.Data;
using ExplogineMonoGame.Layout;
using Microsoft.Xna.Framework;
using Xunit;

namespace ExplogineMonoGameTests;

public class TestLayout
{
    [Fact]
    public void build_serial_from_real_layout()
    {
        var layout = Layout.Create(
            new RectangleF(0, 0, 500, 500),
            new LayoutElementGroup(
                new ArrangementSettings(
                    Orientation.Vertical,
                    Margin: new Vector2(25, 25)),
                new[]
                {
                    Layout.Group(Layout.FillHorizontal("title-bar", 40),
                        new ArrangementSettings(
                            Alignment: Alignment.Center),
                        new[]
                        {
                            Layout.FixedElement("icon", 32, 32),
                            Layout.Group(Layout.FillHorizontal(32),
                                new ArrangementSettings(
                                    Alignment: Alignment.CenterRight,
                                    PaddingBetweenElements: 3),
                                new[]
                                {
                                    Layout.FillHorizontal("title", 32),
                                    Layout.FixedElement("minimize-button", 32, 32),
                                    Layout.FixedElement("fullscreen-button", 32, 32),
                                    Layout.FixedElement("close-button", 32, 32)
                                })
                        }),
                    Layout.FillBoth("body")
                }));

        Approvals.Verify(LayoutSerialization.ToJson(layout));
    }

    [Fact]
    public void build_layout_from_serial()
    {
    }
}
