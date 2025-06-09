using Glouton.Interfaces;
using Glouton.Utils.RetryPolicies;

namespace Glouton.Features.FileManagement.FileDeletion;

public class FileSystemDeletionFactory : IFileSystemDeletionFactory
{
    public IFileSystemDeletion CreateDeletionWithExponentialRetry()
    {
        return new FileSystemDeletion(
            new FileDeletionProxy(),
            new DirectoryDeletionProxy(),
            new ExponentialRetry()
        );
    }

    public IFileSystemDeletion CreateDeletionWithFixedRetry()
    {
        return new FileSystemDeletion(
            new FileDeletionProxy(),
            new DirectoryDeletionProxy(),
            new ExponentialRetry()
        );
    }
}