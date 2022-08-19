// See https://aka.ms/new-console-template for more information

using ExplogineCore;

var commandLineArguments = new ParsedCommandLineArguments(args);

commandLineArguments.RegisterParameter<string>("name");

if (commandLineArguments.GetValue<bool>("help"))
{
    Console.WriteLine(commandLineArguments.HelpOutput());
}