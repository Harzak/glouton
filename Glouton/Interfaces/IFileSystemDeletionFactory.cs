namespace Glouton.Interfaces;

public interface IFileSystemDeletionFactory
{
    public IFileSystemDeletion CreateDeletionWithExponentialRetry();
    public IFileSystemDeletion CreateDeletionWithFixedRetry();
}
