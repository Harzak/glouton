using System;

namespace Glouton.Features.FileManagement.FileDetection;

/// <summary>
/// Immutable record containing essential metadata about a file at a specific point in time.
/// Used for tracking changes to files and determining if a file has been modified or recreated.
/// </summary>
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