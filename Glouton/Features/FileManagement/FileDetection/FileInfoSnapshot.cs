using System;

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