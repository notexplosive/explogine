using ExplogineDesktop;
using ExplogineMonoGame;
using Microsoft.Xna.Framework;
using SampleGame;

var config = new WindowConfigWritable
{
    WindowSize = new Point(1600, 900),
    Title = "Example Cartridge"
};
Bootstrap.Run(args, new WindowConfig(config), runtime => new SampleGameCartridge(runtime));
