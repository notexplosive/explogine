namespace ExplogineMonoGame.Gui;

internal interface IThemed
{
    IGuiTheme Theme { get; }
}

internal interface IPreDrawWidget
{
    public void PrepareDraw(Painter painter, IGuiTheme uiTheme);
}
