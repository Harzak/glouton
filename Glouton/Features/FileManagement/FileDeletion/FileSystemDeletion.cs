using Glouton.Interfaces;
using Glouton.Utils.Result;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Glouton.Features.FileManagement.FileDeletion;

internal class FileSystemDeletion : IFileSystemDeletion
{
    private readonly IFileSystemDeletionProxy _fileDeletion;
    private readonly IFileSystemDeletionProxy _directoryDeletion;
    private readonly IRetryPolicy _retryPolicy;
    private readonly OperationResult _result;

    public FileSystemDeletion(IFileSystemDeletionProxy fileDeletion, IFileSystemDeletionProxy directoryDeletion, IRetryPolicy retryPolicy)
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

        if (File.Exists(path))
        {
            await TryDeleteAsFileAsync(path).ConfigureAwait(false);
        }
        else if (Directory.Exists(path))
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
        for (int attempt = 0; attempt < _retryPolicy.MaxAttemps; attempt++)
        {
            OperationResult result = this.DeleteFileSystemEntry(path, _fileDeletion);

            if (result.IsSuccess)
            {
                _result.WithSuccess();
                return;
            }
            else
            {
                TimeSpan delay = _retryPolicy.GetNextRetryDelay(attempt);
                await Task.Delay(delay).ConfigureAwait(false);
            }
        }
        _result.WithError($"Deletion failed after {_retryPolicy.MaxAttemps} attempts for {path}.");
    }

    private async Task TryDeleteAsDirectoryAsync(string path)
    {
        string[] nesteadDirectories = Directory.GetDirectories(path);
        string[] files = Directory.GetFiles(path);

        if (files != null && files.Length == 0)
        {
            foreach (string file in files.Where(x => x != null))
            {
                await TryDeleteAsFileAsync(file).ConfigureAwait(false);
            }
        }

        if (nesteadDirectories != null && nesteadDirectories.Length == 0)
        {
            foreach (var nesteadDirectory in nesteadDirectories.Where(x => x != null))
            {
                await TryDeleteAsDirectoryAsync(nesteadDirectory).ConfigureAwait(false);
            }
        }

        for (int attempt = 0; attempt < _retryPolicy.MaxAttemps; attempt++)
        {
            OperationResult result = this.DeleteFileSystemEntry(path, _directoryDeletion);

            if (result.IsSuccess)
            {
                _result.WithSuccess();
                return;
            }
            else
            {
                TimeSpan delay = _retryPolicy.GetNextRetryDelay(attempt);
                await Task.Delay(delay).ConfigureAwait(false);
            }
        }
    }

    private OperationResult DeleteFileSystemEntry(string path, IFileSystemDeletionProxy deletionProxy)
    {
        OperationResult result = new();
        try
        {
            deletionProxy.Delete(path);

            if (!deletionProxy.Exists(path))
            {
                return result.WithSuccess();
            }
        }
        catch (UnauthorizedAccessException)
        {
            return result.WithError($"Deletion failed: Unauthorized access for {path}.");
        }
        catch (IOException)
        {
            return result.WithError($"Deletion failed: IO error for {path}.");
        }
        catch (Exception)
        {
            result.WithError($"Deletion failed with unexpected error for {path}.");
            throw;
        }

        return result.WithError("Deletion failed: unknown error.");
    }
}