using System.Threading.Tasks;

namespace Glouton.Utils.TaskScheduling;

internal sealed class SingleTaskScheduler : BaseTaskScheduler
{
    private static TaskScheduler? _inner;

    public static new TaskScheduler Current => _inner ??= new SingleTaskScheduler();

    private SingleTaskScheduler()
        : base(maxConcurrencyLevel: 1)
    {

    }
}