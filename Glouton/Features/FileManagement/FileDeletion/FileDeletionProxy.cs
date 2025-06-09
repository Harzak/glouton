using Glouton.Interfaces;
using System.IO;

namespace Glouton.Features.FileManagement.FileDeletion;

internal class FileDeletionProxy : IFileSystemDeletionProxy
{
    public void Delete(string path)
    {
        File.SetAttributes(path, FileAttributes.Normal);
        File.Delete(path);
    }

    public bool Exists(string path) => File.Exists(path);
}