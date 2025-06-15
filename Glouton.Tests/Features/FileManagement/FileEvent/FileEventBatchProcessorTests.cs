using FakeItEasy;
using FluentAssertions;
using Glouton.Features.FileManagement.FileEvent;
using Glouton.Interfaces;
using System.Timers;

namespace Glouton.Tests.Features.FileManagement.FileEvent;

[TestClass]
public class FileEventBatchProcessorTests
{
    private ILoggingService _loggingService;
    private Interfaces.ITimer _timer;

    [TestInitialize]
    public void Initialize()
    {
        _loggingService = A.Fake<ILoggingService>();
        _timer = A.Fake<Interfaces.ITimer>();
    }

    [TestMethod]
    public void Batch_ShouldBeProcessed_WhenTimerElpased()
    {
        //Arrange
        bool isProcessed = false;
        var proc = new FileEventBatchProcessor((model) => { isProcessed = true; }, maxBatchItem: 1, _loggingService, _timer);
        FileEventActionModel model = CreateFileEventActionModel();

        //Act
        proc.Enqueue(model);
        _timer.Elapsed += Raise.FreeForm<ElapsedEventHandler>.With(_timer, null);

        //Assert
        isProcessed.Should().BeTrue();
    }

    [TestMethod]
    public void FileEvents_ShouldBeProcessed_ByBatch()
    {
        //Arrange
        int batchNumber = 10;
        int maxBatchItem = 2;
        int batchCount = 0;
        var proc = new FileEventBatchProcessor((model) => { batchCount++; }, maxBatchItem, _loggingService, _timer);
        IEnumerable<FileEventActionModel> models = CreateMultipleFileEventActionMode(batchNumber * maxBatchItem);

        //Act
        for (int i = 0; i < models.Count(); i++)
        {
            proc.Enqueue(models.ElementAt(i));
        }
        for (int i = 0; i < batchNumber; i++)
        {
            _timer.Elapsed += Raise.FreeForm<ElapsedEventHandler>.With(_timer, null);
        }

        //Assert
        batchCount.Should().Be(batchNumber);
    }

    [TestMethod]
    public void Enqueue_Should_StartTimer()
    {
        //Arrange
        int nbItem = 10;
        var proc = new FileEventBatchProcessor((model) => { },  maxBatchItem:1, _loggingService, _timer);
        IEnumerable<FileEventActionModel> models = CreateMultipleFileEventActionMode(nbItem);

        //Act
        for (int i = 0; i < models.Count(); i++)
        {
            proc.Enqueue(models.ElementAt(i));
        }

        //Assert
        A.CallTo(() => _timer.StartTimer()).MustHaveHappened(nbItem, Times.Exactly);
    }

    private IEnumerable<FileEventActionModel> CreateMultipleFileEventActionMode(int nulber)
    {
        List<FileEventActionModel> models = new();
        for (int i = 0; i < nulber; i++)
        {
            models.Add(CreateFileEventActionModel());
        }
        return models;
    }

    private FileEventActionModel CreateFileEventActionModel()
    {
        return new FileEventActionModel(CancellationToken.None)
        {
            Id = Guid.NewGuid(),
            Action = () => { }
        };
    }
}