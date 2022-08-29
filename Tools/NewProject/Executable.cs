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
        Phase("Checking parameters", () => CollectParameters(commandLineParameters));
        Phase("Creating directory", () =>
        {
            var projectName = commandLineParameters.Args.GetValue<string>("name");
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
                Path.Join(projectDirectory, $"{commandLineParameters.Args.GetValue<string>("name")}.sln")
            );
        });
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
            _terminated = true;
            foreach (var error in _allErrors)
            {
                _logger.Error(error);
            }
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
            if(!commandLineParameters.Args.HasValue(paramName))
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
