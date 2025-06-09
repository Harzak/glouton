using System;
using System.IO;
using System.Threading;

namespace Glouton.Features.FileManagement.FileEvent;

internal sealed class FileEventActionModel 
{
    public required Guid Id { get; set; }
    public FileSystemEventArgs? EventArgs { get; set; }
    public string FileName => this.EventArgs?.Name ?? "<None>"; 
    public required Action Action { get; set; }
    public CancellationToken CancellationToken { get; set; }

    public FileEventActionModel(CancellationToken cancellationToken)
    {
        CancellationToken = cancellationToken;
    }
}