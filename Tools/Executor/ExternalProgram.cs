using System.ComponentModel;
using System.Diagnostics;

namespace Executor;

public class ExternalProgram
{
    private readonly ILogger _logger;
    private readonly string _exePath;
    private readonly string _workingDirectory;

    public ExternalProgram(string exePath, string workingDirectory, ILogger logger)
    {
        _workingDirectory = workingDirectory;
        _exePath = exePath;
        _logger = logger;
    }

    public ProgramOutput RunWithArgs(ProgramOutputLevel outputLevel, params string[] argumentList)
    {
        return RunWithArgsAt(_workingDirectory, outputLevel, argumentList);
    }

    public ProgramOutput RunWithArgsAt(string workingDirectory, ProgramOutputLevel outputLevel,
        params string[] argumentList)
    {
        if (outputLevel == ProgramOutputLevel.AllowToConsole)
        {
            _logger.Info("ran command: " + _exePath + (argumentList.Length > 0 ? " " : "") +
                         string.Join(" ", argumentList)
                         + "\n" + "in working directory: " + workingDirectory);
        }

        var wasSuccessful = true;
        using (var process = new Process())
        {
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.FileName = _exePath;
            process.StartInfo.UseShellExecute = false;

            if (outputLevel == ProgramOutputLevel.SuppressFromConsole)
            {
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
            }

            foreach (var argument in argumentList)
            {
                process.StartInfo.ArgumentList.Add(argument);
            }

            try
            {
                process.Start();

                if (outputLevel == ProgramOutputLevel.SuppressFromConsole)
                {
                    // Flush the buffers
                    process.StandardOutput.ReadToEnd();
                    process.StandardError.ReadToEnd();
                }

                process.WaitForExit();
            }
            catch (Win32Exception)
            {
                wasSuccessful = false;
            }
        }

        return new ProgramOutput(wasSuccessful);
    }

    private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        Console.Write(e.Data);
    }
}
