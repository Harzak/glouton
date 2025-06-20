using FakeItEasy;
using FluentAssertions;
using Glouton.EventArgs;
using Glouton.Features.Glouton;
using Glouton.Interfaces;
using Glouton.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Tests.UnitTests.Features.Glouton;

[TestClass]
public class HungryGloutonTests
{
    private const string WATCHED_FILEPATH = "C:\\Temp";

    private IFileDetection _detection;
    private IFileSystemDeletionFactory _deletionFactory;
    private IFileSystemDeletion _deletion;
    private ISettingsService _settingsService;
    private ILoggingService _logger;

    [TestInitialize]
    public void Initialize()
    {
        _detection = A.Fake<IFileDetection>();
        _deletionFactory = A.Fake<IFileSystemDeletionFactory>();
        _settingsService = A.Fake<ISettingsService>();
        _logger = A.Fake<ILoggingService>();
        _deletion = A.Fake<IFileSystemDeletion>();

        A.CallTo(() => _deletionFactory.CreateDeletionWithExponentialRetry()).Returns(_deletion);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _detection?.Dispose();
    }

    [DataRow("testfile.txt")]
    [DataRow("testfile.cs")]
    [DataRow("testfile.py")]
    [TestMethod]
    public void EatingGoodThings_MakesGlouton_Happy(string file)
    {
        //Arrange
        string filePath = Path.Combine(WATCHED_FILEPATH, file); 
        using HungryGlouton glouton = new( _detection, _deletionFactory, _settingsService, _logger);
        A.CallTo(() => _deletion.StartAsync(filePath)).Returns(new Utils.Result.OperationResult(success: true));

        //Act
        glouton.WakeUp();
        _detection.FileDetected += Raise.FreeForm<EventHandler<DetectedFileEventArgs>>.With(_detection, new DetectedFileEventArgs(filePath, DateTime.UtcNow));

        //Assert
        glouton.HungerLevel.Should().BeGreaterThan(HungryGlouton.DEFAULT_HUNGER_LEVEL); 
    }

    [DataRow("testfile.exe")]
    [DataRow("testfile.dll")]
    [DataRow("testfile.iso")]
    [TestMethod]
    public void EatingBadThings_MakesGlouton_Sad(string file)
    {
        //Arrange
        string filePath = Path.Combine(WATCHED_FILEPATH, file);
        using HungryGlouton glouton = new(_detection, _deletionFactory, _settingsService, _logger);
        A.CallTo(() => _deletion.StartAsync(filePath)).Returns(new Utils.Result.OperationResult(success: true));

        //Act
        glouton.WakeUp();
        _detection.FileDetected += Raise.FreeForm<EventHandler<DetectedFileEventArgs>>.With(_detection, new DetectedFileEventArgs(filePath, DateTime.UtcNow));

        //Assert
        glouton.HungerLevel.Should().BeLessThan(HungryGlouton.DEFAULT_HUNGER_LEVEL);
    }

}