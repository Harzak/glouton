using Glouton.EventArgs;
using System;
using System.IO;
using System.Threading;

namespace Glouton.Features.FileManagement.FileEvent;

/// <summary>
/// Model class that encapsulates a file event action to be executed.
/// Contains the file event arguments, the action to perform, and 
/// cancellation support for controlling the action's execution.
/// </summary>
public sealed class FileEventActionModel
{
    public DetectedFileEventArgs? EventArgs { get; set; }
    public string FileName => Path.GetFileName(this.EventArgs?.FilePath) ?? "<None>";
    public required Action Action { get; set; }
    public CancellationToken CancellationToken { get; set; }

    public FileEventActionModel(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
    }
}