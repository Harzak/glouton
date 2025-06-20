using Glouton.Interfaces;
using Glouton.Utils.RetryPolicies;
using Glouton.Wrappers;
using System;

namespace Glouton.Features.FileManagement.FileDeletion;

public class FileSystemDeletionFactory : IFileSystemDeletionFactory
{
    private readonly IFileSystemFacade _fileFacade;
    private readonly IDirectoryFacade _directoryFacade;

    public FileSystemDeletionFactory(IFileSystemFacade fileFacade, IDirectoryFacade directoryFacade)
    {
        _fileFacade = fileFacade;
        _directoryFacade = directoryFacade;
    }

    public IFileSystemDeletion CreateDeletionWithExponentialRetry()
    {
        return new FileSystemDeletion(
            _fileFacade,
            _directoryFacade,
            new ExponentialRetry()
        );
    }

    public IFileSystemDeletion CreateDeletionWithFixedRetry(TimeSpan timeToWait)
    {
        return new FileSystemDeletion(
            _fileFacade,
            _directoryFacade,
            new FixedRetry(timeToWait)
        );
    }
}