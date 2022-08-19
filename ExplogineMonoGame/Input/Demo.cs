using System;
using System.Collections.Generic;
using System.Text;

namespace ExplogineMonoGame.Input;

public class Demo
{
    private readonly List<InputSnapshot> _records = new();

    private DemoState _demoState = DemoState.Stopped;
    private int _playHeadIndex;

    internal Demo()
    {
    }

    public bool IsRecording => _demoState == DemoState.Recording;
    public bool IsPlaying => _demoState == DemoState.Playing;

    public void BeginRecording()
    {
        _records.Clear();
        _demoState = DemoState.Recording;
    }

    public void AppendRecording()
    {
        _demoState = DemoState.Recording;
    }

    public void Stop()
    {
        _demoState = DemoState.Stopped;
    }

    public void BeginPlayback()
    {
        _playHeadIndex = 0;
        _demoState = DemoState.Playing;
    }

    public void AddRecord(InputSnapshot humanSnapshot)
    {
        _records.Add(humanSnapshot);
    }

    public void DumpRecording()
    {
        var stringBuilder = new StringBuilder();
        string? mostRecent = null;
        var duplicateCount = 0;
        foreach (var record in _records)
        {
            var serial = record.Serialize();
            if (mostRecent == serial)
            {
                duplicateCount++;
            }
            else
            {
                if (duplicateCount > 0)
                {
                    stringBuilder.AppendLine($"wait:{duplicateCount}");
                    duplicateCount = 0;
                }

                stringBuilder.AppendLine(serial);
            }

            mostRecent = serial;
        }

        Client.FileSystem.WriteFileToWorkingDirectory("default.demo", stringBuilder.ToString());
    }
    
    public void LoadFile(string path)
    {
        var file = Client.FileSystem.ReadTextFileInWorkingDirectory(path);
        file.Wait();
        LoadText(file.Result);
    }

    private void LoadText(string text)
    {
        var startIndex = 0;
        var length = 0;
        InputSnapshot mostRecent = new InputSnapshot();
        
        for (int currentIndex = 0; currentIndex < text.Length; currentIndex++)
        {
            bool isAtNewline = true;

            for(int offset = 0; offset < Environment.NewLine.Length; offset++)
            {
                var currentIndexWithOffset = currentIndex + offset;
                if (text.Length <= currentIndexWithOffset)
                {
                    isAtNewline = false;
                    break;
                }
                if (text[currentIndexWithOffset] != Environment.NewLine[offset])
                {
                    isAtNewline = false;
                }
            }

            length++;
            
            if (isAtNewline)
            {
                var line = text.Substring(startIndex, length);

                if (line.StartsWith("wait"))
                {
                    var waitFrames = int.Parse(line.Split(':')[1]);
                    for (int i = 0; i < waitFrames; i++)
                    {
                        _records.Add(mostRecent);
                    }
                }
                else
                {
                    mostRecent = new InputSnapshot(line);
                    _records.Add(mostRecent);
                }

                startIndex = currentIndex + Environment.NewLine.Length;
                length = 0;
            }
        }
    }

    public InputSnapshot GetNextRecordedState()
    {
        if (_records.Count > _playHeadIndex)
        {
            return _records[_playHeadIndex++];
        }

        Stop();
        
        // If we just hit the end of the recording, feed in the latest human input
        return InputSnapshot.Human;
    }

    private enum DemoState
    {
        Stopped,
        Recording,
        Playing
    }
}
