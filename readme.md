# Explogine

Explogine is NotExplosive's wrapper around MonoGame to abstract away MonoGame's paradigms into something that's a bit nicer to work with `[citation needed]`.

## Caution: Incomplete Software Ahead

Explogine is still under development. At time of writing I use Explogine regularly and make frequent additions to it as I discover features that are missing. That being said, it already has a lot of features that are very cool and worth having.

## Cool Features

- Development Screenshots on every build
  - You can turn this off by passing `--skipSnapshot` into the command line args of your project.
  - This only affects debug builds
  - Upon running your game, explogine automatically takes a screenshot of the current game canvas. It takes more screenshots periodically after that. This lets you create really cool timelapses for game jams.
- Demo Recording
  - You can record a series of inputs (mouse movements, button presses, etc) and play them back. Useful for debugging!
  - If you pass `--demo=record` into the command line args of your project, you'll start recording a demo
  - You can then press `CTRL P` in game to save the demo
  - Then, if you pass `--demo=playback` in a subsequent run, the game will playback your inputs recorded in the demo
- Frame Stepping
  - In a debug build you can press `CTRL Space` to freeze the update loop
  - You can then scroll down on the scroll wheel to step forward one frame.
- Crash Handling
  - If an exception is ever thrown at runtime, Explogine presents a crash screen reporting the thrown exception. (which beats the alternative of stopping the process)
  - This only effects Release builds, so the debug build debugging experience is not effected.
  - It also drops a log file that includes everything that was logged to the console that run
- Debug Mode
  - Pressing `CTRL SHIFT ~` will cycle your debug mode. `Off -> Passive -> Active -> Passive`
  - Debug builds starts out as `Passive`
  - Release starts out as `Off`
  - `Passive` enables debug features like Frame Step and the Developer Console
  - `Active` enables more "aggressive" debug features (ie: making collision boxes visible)

# Quickstart

If you want to get started with an Explogine project, the easiest way to do that is using the `NewProject` tool. You _can_ just manually import all the `csproj` and `shproj` files into a solution and work from there. But `NewProject` gets you setup quickly.

- Install .NET 6 make sure that `dotnet --version` reports `6.0.xxx`
- Clone this repo (for this example we'll say it's cloned to `D:\dev\explogine`)
- Enter a terminal in the folder you want to create your project in (we'll put our terminal at `D:\dev`, with the intention to create `D:\dev\MyProject`)
- Run `dotnet run --project D:\dev\csharp\explogine\Tools\NewProject\NewProject.csproj -- --name=MyProject`
  - This builds Explogine's `NewProject` project, and then invokes it with the args: `--name=MyProject`
- Open the newly created sln (in this example that would end up at `D:\dev\MyProject\MyProject.sln`)
- Open `Program.cs`
- Create a class called `MyCartridge` that derives from `ICartridge` and replace the `BlankCartridge` with it.

```
Bootstrap.Run(args, new WindowConfig(config), new BlankCartridge());
                                                  ^^^^^^^^^^^^^^
```

```
Bootstrap.Run(args, new WindowConfig(config), new MyCartridge());
```

- Implement `ICartridge` by doing the following:
  - `CartridgeConfig => new()` <-- Feel free to put parameters here, but empty is fine
  - Leave `OnCartridgeStarted` and `Update` blank
  - `ShouldLoadNextCartridge() => false;`
  - `Draw` should look like the following:

```cs
public void Draw(Painter painter)
{
    // 1
    painter.BeginSpriteBatch(SamplerState.LinearWrap);

    // 2
    painter.DrawStringWithinRectangle(
        // 3
        Client.Assets.GetFont("engine/console-font", 40),
        "Hello world!",
        new Rectangle(
            Point.Zero,
            // 4
            Client.Window.RenderResolution),
        // 5
        Alignment.Center,
        // 6
        new DrawSettings {Color = Color.White}
    );

    // 7
    painter.EndSpriteBatch();
}
```

1. We cannot draw anything until we tell the `Painter` to Begin its spriteBatch. This is basically 1:1 with MonoGame's `SpriteBatch.Begin` only with fewer parameters.
2. `Painter` has a number of convenient methods for drawing. In this case, we're using `DrawStringWithinRectangle` which renders some text that is linebroken within a rectangle. In this case we're providing a giant rectangle (the whole viewable space!) and then using `Alignment` to center it within that rectangle.
3. We obtain the `engine/console-font` Font from `Client.Assets`, this is guaranteed to be available because explogine uses it for rendering the developer console and the loading screen
4. We ask `Client.Window` for the render resolution. This gives us back the current `RenderResolution` for the cartridge. If you customized it in `CartridgeConfig`, you'll get back the value you put in. Not that `RenderResolution` is not the same as `Client.Window.Size` which is the size of the window.
5. `Alignment` is a struct that behaves like an enum.
6. All of the `Painter`'s draw methods take a `DrawSettings`, this is a property bag of optional parameters. Often times you can leave this blank and get an OK result, the intended use to is to call it using the `{}` braced initialization to pick and choose the exact values you want.
7. We have to end the `SpriteBatch` before anything renders, just like MonoGame normally works.

## What is a Cartridge?

Cartridges are an abstraction introduced by Explogine to make it easier to load several distinct "Game-Type-Things" in sequence. By default, there are 3 cartridges loaded in Explogine:

`[Loading Screen Cartridge] -> [Intro Screen Cartridge] -> [Your Cartridge]`

- Upon startup, we query each cartridge for any command line parameters it wants to register, and we queue up any `LoadEvents` they want to register.
- We parse the registered command lines against the provided `args[]` by the user and set them aside.
- Then we load the `[Loading Screen Cartridge]` which executes all loading events (this includes `Content` loading).
  - See `LoadingCartridge.cs` for details
- Once loading is finished `[Loading Screen Cartridge]` yields control over to the next cartridge in the chain, `[Intro Screen Cartridge]`
- `[Intro Screen Cartridge]` is an optional cartridge that displays `NotExplosive.net` in a randomly selected animation. You're free to remove this if you don't want it.
- We then load `[Your Cartridge]`, which is the cartridge you provided to the `Bootstrap.Run` method.

## How to make a Game Cartridge

A "Cartridge" just needs to implement `ICartridge`, so in short, you just need to implement `ICartridge` and pass the cartridge into `Bootstrap.Run` and the following happens for free:

- Your `CartridgeConfig` will be set upon loading your cartridge.
  - At time of writing CartridgeConfig just contains `RenderResolution`
- `OnCartridgeStarted` will be run right after setting `CartridgeConfig`, it will be run exactly once.
- `Update` will run every frame
- `Draw` will be run every frame, just after Update (same as MonoGame normally does it)
- `ShouldLoadNextCartridge` will pop this cartridge and load the next one in the chain when true.
  - Unless you're actually taking advantage of the `Cartridge` subsystem, you should **always** return `false` from `ShouldLoadNextCartridge`

Cartridges have two interfaces they can optionally implement to get more functionality.

- `ILoadEventProvider` requires implementing a method that returns an `IEnumerable<LoadEvent?>`. Each LoadEvent yielded will be executed at the loading screen. It will be made available in `Client.Assets` under the name you provided in the `LoadEvent`
- `ICommandLineParameterProvider` requires implementing a method that takes an `CommandLineParametersWriter`, with this object you can register command line parameters that you can use with the format `--parameterName=value`. If you register a parameter in this method, it will be availble in `Client.Args.GetValue<T>(parameterName)`.
  - Only string, int, float, and bool are supported parameter types.
  - If you invoke a parameter without an equals sign (eg: `--parameterName`) it assumes this is a boolean value and it parsed as `--parameterName=true`
  - Command line parameter names are case insensitive (eg: `--parameterName` is the same is `--parametername`)

Note: you can also derive your `Cartridge` from the abstract class `BasicGameCartridge` to implement these for free (it also implements `ShouldLoadNextCartridge` for you)

## Content Management

One of the most notorious features of MonoGame is the Content Pipeline. Explogine attempts to abstract away the need for the Content Pipeline tool by building your `mgcb` file for you. This means I make some assumptions about what you want your Content to look like and I might not get it right (also my tooling is incomplete). So you might still be better off using that awful pipeline GUI.

That being said, once your build-time content is setup, Explogine does load it for you and put it in a convenient place.

- If you used the `NewProject` quickstart guide above, you should have a folder called `Assets` which contains a folder called `Content`
- Say inside this folder you have a png file at a path like `.\characters\smily.png` or `./characters/smily.png`
- Assuming `smily.png` is included in your `Content.mgcb` (which will happen automatically if you run `rebuild_content.bat` at the root of your project) it will be available in your project with `Client.Assets.GetTexture("characters/smily")`
  - The path must be with `/` forward slashes, and the extension gets removed.
- This will be a similar story for other MonoGame supported Content types, namely `SoundEffect` and `SoundEffectInstance`.
- `SpriteFonts` are handled a little differently, you'll use `GetFont` which will give you a `Font`, a wrapper around `SpriteFont` that can be scaled to any size.
  - The intended use case here is you create a `SpriteFont` with a very large font size and then scale it down when you use it... because SpriteFont rendering is terrible and this is the only way to get something that looks decent.
- Explogine supports a number of other `Asset` types (and you can implement your own!). You'll want to use `Client.Assets.GetAsset<T>()` to obtain any of those.

## Client Is King

Explogine has one big Singleton object called the `Client`. Normally I don't like using singleton but MonoGame already makes the assumption that there is only ever one `Game` so we may as well codify it.

`Client` is the _only_ Singleton (at least the only one you need to be aware of). Every way you interface with Explogine is through the Client. The main things you'll probably want from Client:

- `Client.Input` - Use `.Keyboard` `.Mouse` and `.GamePad` to obtain the state of any button, trigger, or pointer.
- `Client.Assets` - Asset Library, includes content and other loaded assets.
- `Client.Graphics` - Global Graphics APIs (also gives you access to the `Painter`, but you should probably access the Painter via passing down from `Draw`)
- `Client.FileSystem` - Use to access the FileSystem, you can, of course, use other means, but Client.FileSystem is meant to be platform agnostic, meaning it will eventually work on Android and iOS with the same API.
- `Client.Args` - Use to access the command line args and their values. Must be setup via cartridge implementing `ICommandLineParameterProvider`
- `Client.Random` - Clean, seeded, noise-based randomness
