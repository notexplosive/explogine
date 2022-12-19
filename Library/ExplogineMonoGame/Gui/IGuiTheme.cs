namespace ExplogineMonoGame.Gui;

public interface IGuiTheme
{
    void DrawButton(Painter painter, Button button);
    void DrawPanel(Painter painter, Panel panel);
    void DrawCheckbox(Painter painter, Checkbox checkbox);
    void DrawSlider(Painter painter, Slider slider);
}
