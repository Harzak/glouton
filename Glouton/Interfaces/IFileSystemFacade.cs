namespace Glouton.Interfaces;

public interface IFileSystemFacade
{
    void Delete(string path);
    bool Exists(string path);
}