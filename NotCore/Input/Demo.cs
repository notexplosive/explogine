using System.Collections.Generic;

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
