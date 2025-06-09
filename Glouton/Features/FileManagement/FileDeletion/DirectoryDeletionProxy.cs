using Glouton.Interfaces;
using System.IO;

namespace Glouton.Features.FileManagement.FileDeletion;
internal class DirectoryDeletionProxy : IFileSystemDeletionProxy
{
    public void Delete(string path)
    {
        File.SetAttributes(path, FileAttributes.Normal);
        Directory.Delete(path);
    }
    public bool Exists(string path) => Directory.Exists(path);
}