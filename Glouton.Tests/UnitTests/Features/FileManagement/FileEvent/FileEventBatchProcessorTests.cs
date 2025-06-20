using FakeItEasy;
using FluentAssertions;
using Glouton.Features.FileManagement.FileEvent;
using Glouton.Interfaces;
using System.Timers;

namespace Glouton.Tests.UnitTests.Features.FileManagement.FileEvent;

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
        var proc = new FileEventBatchProcessor(TestsUtils.CreateBatchSettings(maxItems: 1), _loggingService, _timer);
        FileEventActionModel model = TestsUtils.CreateFileEventActionModel();

        //Act
        proc.Initialize((models) => { isProcessed = true; });
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

        var proc = new FileEventBatchProcessor(TestsUtils.CreateBatchSettings(maxItems: maxBatchItem), _loggingService, _timer);
        IEnumerable<FileEventActionModel> models = TestsUtils.CreateMultipleFileEventActionMode(batchNumber * maxBatchItem);

        //Act
        proc.Initialize((models) => { batchCount++; });
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
        var proc = new FileEventBatchProcessor(TestsUtils.CreateBatchSettings(maxItems: nbItem), _loggingService, _timer);
        IEnumerable<FileEventActionModel> models = TestsUtils.CreateMultipleFileEventActionMode(nbItem);

        //Act
        proc.Initialize((models) => { });
        for (int i = 0; i < models.Count(); i++)
        {
            proc.Enqueue(models.ElementAt(i));
        }

        //Assert
        A.CallTo(() => _timer.StartTimer()).MustHaveHappened(nbItem, Times.Exactly);
    }
}