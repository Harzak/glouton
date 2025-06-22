using Glouton.Interfaces;
using Glouton.Utils.Result;
using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Glouton.Tests")]

namespace Glouton.Features.FileManagement.FileDeletion;

/// <summary>
/// Responsible for deleting files and directories with a retry mechanism.
/// Handles both individual file deletion and recursive directory deletion,
/// applying the specified retry policy when operations fail.
/// </summary>
internal sealed class FileSystemDeletion : IFileSystemDeletion
{
    private readonly IFileSystemFacade _fileDeletion;
    private readonly IDirectoryFacade _directoryDeletion;
    private readonly IRetryPolicy _retryPolicy;
    private readonly OperationResult _result;

    public FileSystemDeletion(IFileSystemFacade fileDeletion, IDirectoryFacade directoryDeletion, IRetryPolicy retryPolicy)
    {
        _fileDeletion = fileDeletion;
        _directoryDeletion = directoryDeletion;
        _retryPolicy = retryPolicy;

        _result = new OperationResult();
    }

    public async Task<OperationResult> StartAsync(string path)
    {
        if (string.IsNullOrEmpty(path?.Trim()))
        {
            return _result.WithError("Deletion attempt failed: file is empty.");
        }

        if (_fileDeletion.Exists(path))
        {
            await TryDeleteAsFileAsync(path).ConfigureAwait(false);
        }
        else if (_directoryDeletion.Exists(path))
        {
            await TryDeleteAsDirectoryAsync(path).ConfigureAwait(false);
        }
        else
        {
            _result.WithError($"Deletion attempt failed: {path} cannot be found.");
        }
        return _result;
    }

    private async Task TryDeleteAsFileAsync(string path)
    {
        OperationResult retryResult = new OperationResult();
        for (int attempt = 0; attempt < _retryPolicy.MaxAttemps; attempt++)
        {
            retryResult = this.DeleteFileSystemEntry(path, _fileDeletion);

            if (retryResult.IsFailed)
            {
                TimeSpan delay = _retryPolicy.GetNextRetryDelay(attempt);
                await Task.Delay(delay).ConfigureAwait(false);
            }
            else
            {
                break;
            }
        }
        _result.Affect(retryResult);
    }

    private async Task TryDeleteAsDirectoryAsync(string path)
    {
        string[] nestedDirectories = _directoryDeletion.GetDirectories(path);
        string[] files = _directoryDeletion.GetFiles(path);

        if (files != null && files.Length > 0)
        {
            foreach (string file in files.Where(x => x != null))
            {
                await TryDeleteAsFileAsync(file).ConfigureAwait(false);
            }
        }

        if (nestedDirectories != null && nestedDirectories.Length > 0)
        {
            foreach (var nesteadDirectory in nestedDirectories.Where(x => x != null))
            {
                await TryDeleteAsDirectoryAsync(nesteadDirectory).ConfigureAwait(false);
            }
        }

        OperationResult retryResult = new OperationResult();
        for (int attempt = 0; attempt < _retryPolicy.MaxAttemps; attempt++)
        {
            retryResult = this.DeleteFileSystemEntry(path, _directoryDeletion);

            if (retryResult.IsFailed)
            {
                TimeSpan delay = _retryPolicy.GetNextRetryDelay(attempt);
                await Task.Delay(delay).ConfigureAwait(false);
            }
            else
            {
                break;
            }
        }
        _result.Affect(retryResult);
    }

    private OperationResult DeleteFileSystemEntry(string path, IFileSystemFacade deletionProxy)
    {
        try
        {
            deletionProxy.Delete(path);

            if (!deletionProxy.Exists(path))
            {
                return OperationResult.Success;
            }
        }
        catch (UnauthorizedAccessException)
        {
            return OperationResult.Error($"Deletion failed: Unauthorized access for {path}.");
        }
        catch (IOException)
        {
            return OperationResult.Error($"Deletion failed: IO error for {path}.");
        }
        catch (Exception)
        {
            throw;
        }

        return OperationResult.Error("Deletion failed: unknown error.");
    }
}