using Glouton.Interfaces;
using System.IO;

namespace Glouton.Wrappers;

internal class FileFacade : IFileSystemFacade
{
    public void Delete(string path)
    {
        File.SetAttributes(path, FileAttributes.Normal);
        File.Delete(path);
    }

    public bool Exists(string path) => File.Exists(path);
}