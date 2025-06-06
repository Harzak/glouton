using System.Threading.Tasks;

namespace Glouton.Utils.TaskScheduling;

internal sealed class ParallelTaskScheduler : BaseTaskScheduler
{
    private static TaskScheduler? _inner;

    public static new TaskScheduler Current => _inner ??= new ParallelTaskScheduler();

    private ParallelTaskScheduler()
        : base(maxConcurrencyLevel: 5)
    {

    }
}
