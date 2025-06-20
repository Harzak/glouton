using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Glouton.Features.FileManagement.FileDetection;

public record FileInfoSnapshot
{
    public DateTime CreationTimeUtc { get; init; }
    public DateTime LastWriteTimeUtc { get; init; }
    public long Length { get; init; }

    public FileInfoSnapshot(DateTime creationTimeUtc, DateTime lastWriteTimeUtc, long length)
    {
        this.CreationTimeUtc = creationTimeUtc;
        this.LastWriteTimeUtc = lastWriteTimeUtc;
        this.Length = length;
    }
}