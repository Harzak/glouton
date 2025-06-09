using Glouton.Utils.Result;
using System.Threading.Tasks;

namespace Glouton.Interfaces;

public interface IFileSystemDeletion
{
    Task<OperationResult> StartAsync(string path);
}

