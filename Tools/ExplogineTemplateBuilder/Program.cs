using ExplogineCore;
using ExplogineTemplateBuilder;

var commandLineParameters = new CommandLineParameters(args);

var executable = new Executable();
executable.Run(commandLineParameters);