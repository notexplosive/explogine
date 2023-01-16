using ExplogineDesktop;
using ExplogineMonoGame;
using ExplogineMonoGame.Cartridges;
using Microsoft.Xna.Framework;

var config = new WindowConfigWritable
{
    WindowSize = new Point(1600, 900),
    Title = "NotExplosive.net"
};
Bootstrap.Run(args, new WindowConfig(config), (runtime) => new BlankCartridge(runtime));