using Glouton.Features.FileManagement.FileDetection;
using System;

namespace Glouton.EventArgs;

public class FileDetectionStateEventArgs : System.EventArgs
{
    public EFileDetectionState State { get; }

    public FileDetectionStateEventArgs(EFileDetectionState state)
    {
        State = state;
    }
}