using ExplogineDesktop;
using ExplogineMonoGame;
using Microsoft.Xna.Framework;
using SampleGame;

var config = new WindowConfigWritable
{
    WindowSize = new Point(1600, 900),
    RenderResolution = new Point(1600 / 2, 900 / 2),
    Title = "Example Cartridge"
};
Bootstrap.Run(args, new WindowConfig(config), new SampleGameCartridge());
