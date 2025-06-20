using Glouton.Interfaces;
using System.IO;

namespace Glouton.Wrappers;
internal class DirectoryFacade : IDirectoryFacade
{
    public void Delete(string path)
    {
        File.SetAttributes(path, FileAttributes.Normal);
        Directory.Delete(path);
    }
    public bool Exists(string path) => Directory.Exists(path);

    public string[] GetDirectories(string path) => Directory.GetDirectories(path);

    public string[] GetFiles(string path) => Directory.GetFiles(path);
}