using Executor;
using ExplogineCore;

namespace NewProject;

public class Executable
{
    private readonly List<string> _allErrors = new();
    private readonly ILogger _logger = new HumanFacingConsoleLogger();
    private bool _terminated;

    private IEnumerable<RequiredParameter> RequiredParameters()
    {
        yield return new RequiredParameter<string>("name", "Name of the project you're creating.");
    }

    public void Run(CommandLineParameters commandLineParameters)
    {
        DotnetProgram dotnet = null!;
        GitProgram git = null!;
        string engineDirectory = null!;
        string projectDirectory = null!;
        string projectName = null!;
        Phase("Checking parameters", () => CollectParameters(commandLineParameters));
        Phase("Creating directory", () =>
        {
            projectName = commandLineParameters.Args.GetValue<string>("name");
            Directory.CreateDirectory(projectName);
            projectDirectory = Path.Join(".", projectName);
            git = new GitProgram(projectDirectory, _logger);
            dotnet = new DotnetProgram(projectDirectory, _logger);
        });
        Phase("Initializing git", () => { git.Init(); });
        Phase("Cloning submodule",
            () =>
            {
                git.AddSubmodule(ProgramOutputLevel.SuppressFromConsole, "git@github.com:notexplosive/explogine.git");
                engineDirectory = Path.Join(projectDirectory, "explogine");
            });
        Phase("Copying rebuild_content",
            () =>
            {
                File.Copy(
                    Path.Join(engineDirectory, "rebuild_content_from_game.bat"),
                    Path.Join(projectDirectory, "rebuild_content.bat")
                );
            });
        Phase("Copying .gitignore",
            () =>
            {
                File.Copy(
                    Path.Join(engineDirectory, ".gitignore"),
                    Path.Join(projectDirectory, ".gitignore")
                );
            });
        Phase("Copying template solution", () =>
        {
            File.Copy(
                Path.Join(engineDirectory, "Templates", "Template.sln"),
                Path.Join(projectDirectory, $"{projectName}.sln")
            );
        });
        Phase("Copying Template project", () =>
        {
            CopyDirectory(
                Path.Join(engineDirectory, "Templates", "TemplateGame"),
                Path.Join(projectDirectory, projectName)
            );
        });
        Phase("Copying Assets project", () =>
        {
            CopyDirectory(
                Path.Join(engineDirectory, "Library", "Assets"),
                Path.Join(projectDirectory, "Assets")
            );
        });
        Phase("Renaming copied .csproj", () =>
        {
            File.Move(
                Path.Join(projectDirectory, projectName, "TemplateGame.csproj"),
                Path.Join(projectDirectory, projectName, $"{projectName}.csproj")
            );
        });
        Phase($"Adding project to {projectName}.csproj solution", () =>
        {
            var result = dotnet.AddToSln(ProgramOutputLevel.SuppressFromConsole, projectName);
            if (!result.WasSuccessful)
            {
                Error($"Failed to add {projectName} to sln");
            }
        });
        Phase("Writing initial commit", () =>
        {
            git.AddAll(ProgramOutputLevel.SuppressFromConsole);
            git.CommitWithMessage(ProgramOutputLevel.SuppressFromConsole,"(automated) Initial Commit");
        });
        
        LogErrors();
    }

    private void CopyDirectory(string source, string destination)
    {
        var sourceDirectory = new DirectoryInfo(source);

        if (!sourceDirectory.Exists)
        {
            Error($"Cannot copy from {sourceDirectory.FullName} because it does not exist");
            return;
        }

        var subDirectories = sourceDirectory.GetDirectories();
        Directory.CreateDirectory(destination);

        foreach (var file in sourceDirectory.GetFiles())
        {
            var targetFilePath = Path.Combine(destination, file.Name);
            file.CopyTo(targetFilePath);
        }

        foreach (var subDirectory in subDirectories)
        {
            CopyDirectory(subDirectory.FullName, Path.Combine(destination, subDirectory.Name));
        }
    }

    private void Phase(string message, Action phase)
    {
        if (_terminated)
        {
            return;
        }

        if (_allErrors.Count == 0)
        {
            _logger.Info(message);
            phase();
        }
        else
        {
            LogErrors();
        }
    }

    private void LogErrors()
    {
        _terminated = true;
        foreach (var error in _allErrors)
        {
            _logger.Error(error);
        }
    }

    private void CollectParameters(CommandLineParameters commandLineParameters)
    {
        var requiredParamNames = new List<string>();

        foreach (var param in RequiredParameters())
        {
            param.Bind(commandLineParameters.Writer);
            requiredParamNames.Add(param.Name);
        }

        foreach (var paramName in requiredParamNames)
        {
            if (!commandLineParameters.Args.HasValue(paramName))
            {
                Error($"Missing parameter {paramName}");
            }
        }
    }

    private void Error(string errorMessage)
    {
        _allErrors.Add(errorMessage);
    }
}
