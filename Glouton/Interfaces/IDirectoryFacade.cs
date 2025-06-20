namespace Glouton.Interfaces;

public interface IDirectoryFacade : IFileSystemFacade
{
    string[] GetDirectories(string path);
    string[] GetFiles(string path);
}