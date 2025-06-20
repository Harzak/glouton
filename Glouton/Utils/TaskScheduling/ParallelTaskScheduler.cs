using System.Threading.Tasks;

namespace Glouton.Utils.TaskScheduling;

internal sealed class ParallelTaskScheduler : BaseTaskScheduler
{
    private static TaskScheduler? _inner;

    internal static new TaskScheduler Current => _inner ??= new ParallelTaskScheduler();

    internal ParallelTaskScheduler()
        : base(maxConcurrencyLevel: 5)
    {

    }
}