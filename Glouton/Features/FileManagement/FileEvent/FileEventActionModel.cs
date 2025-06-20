using Glouton.EventArgs;
using System;
using System.IO;
using System.Threading;

namespace Glouton.Features.FileManagement.FileEvent;

public sealed class FileEventActionModel
{
    public required Guid Id { get; set; }
    public DetectedFileEventArgs? EventArgs { get; set; }
    public string FileName => Path.GetFileName(this.EventArgs?.FilePath) ?? "<None>";
    public required Action Action { get; set; }
    public CancellationToken CancellationToken { get; set; }

    public FileEventActionModel(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
    }
}