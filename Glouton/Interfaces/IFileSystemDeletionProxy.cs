namespace Glouton.Interfaces;

public interface IFileSystemDeletionProxy
{
    void Delete(string path);
    bool Exists(string path);
}