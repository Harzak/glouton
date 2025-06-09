using System;
using System.IO;
using System.Threading;

namespace Glouton.Features.FileManagement.FileEvent;

internal sealed class FileEventActionModel 
{
    public Guid Id { get; set; }
    public FileSystemEventArgs? EventArgs { get; set; }
    public string FileName => this.EventArgs?.Name ?? "<None>"; 
    public Action Action { get; set; }
    public CancellationToken CancellationToken { get; set; }

    public FileEventActionModel(Guid id, Action action, CancellationToken cancellationToken)
    {
        Id = id;
        Action = action;
        CancellationToken = cancellationToken;
    }
}