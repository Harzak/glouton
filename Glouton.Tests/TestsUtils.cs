using Glouton.Features.FileManagement.FileEvent;
using Glouton.Settings;
using Microsoft.Extensions.Options;

namespace Glouton.Tests;

internal static class TestsUtils
{
    public static IEnumerable<FileEventActionModel> CreateMultipleFileEventActionMode(int nulber)
    {
        List<FileEventActionModel> models = new();
        for (int i = 0; i < nulber; i++)
        {
            models.Add(CreateFileEventActionModel());
        }
        return models;
    }

    public static FileEventActionModel CreateFileEventActionModel()
    {
        return new FileEventActionModel(CancellationToken.None)
        {
            Id = Guid.NewGuid(),
            Action = () => { }
        };
    }

    public static IOptions<BatchSettings> CreateBatchSettings(int maxItems)
    {
        return Options.Create(new BatchSettings()
        {
            MaxItems = maxItems
        });
    }
}