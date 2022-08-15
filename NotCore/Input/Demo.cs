using System;
using System.Collections.Generic;
using System.Text;

namespace NotCore.Input;

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
        foreach (var record in _records)
        {
            stringBuilder.AppendLine(record.Serialize());
        }

        Client.FileSystem.WriteFileToWorkingDirectory("default.demo", stringBuilder.ToString());
    }
    
    public void LoadFile(string path)
    {
        var file = Client.FileSystem.ReadTextFileInWorkingDirectory(path);
        file.Wait();
        LoadText(file.Result);
    }

    private void LoadText(string data)
    {
        var startIndex = 0;
        var length = 0;
        for (int currentIndex = 0; currentIndex < data.Length; currentIndex++)
        {
            bool isAtNewline = true;

            for(int offset = 0; offset < Environment.NewLine.Length; offset++)
            {
                var currentIndexWithOffset = currentIndex + offset;
                if (data.Length <= currentIndexWithOffset)
                {
                    isAtNewline = false;
                    break;
                }
                if (data[currentIndexWithOffset] != Environment.NewLine[offset])
                {
                    isAtNewline = false;
                }
            }

            length++;
            
            if (isAtNewline)
            {
                var serializedData = data.Substring(startIndex, length);
                _records.Add(new InputSnapshot(serializedData));
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
