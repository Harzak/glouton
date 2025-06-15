using FakeItEasy;
using FluentAssertions;
using Glouton.Features.FileManagement.FileDeletion;
using Glouton.Features.FileManagement.FileEvent;
using Glouton.Features.FileManagement.FileWatcher;
using Glouton.Features.Glouton;
using Glouton.Features.Loging;
using Glouton.Features.Menu;
using Glouton.Interfaces;
using Glouton.Settings;
using Glouton.Utils.Time;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Tests.IntegrationTests;

[TestClass]
public class FileWatcherTests
{
    private DirectoryInfo _directoryToWatch;
    private ServiceProvider _serviceProvider;
    private IServiceScope _scope;

    public TestContext TestContext { get; set; }

    [TestInitialize]
    public void Initialize()
    {
        ConfigureServices();

        string testDataFile = Path.Combine(TestContext.DeploymentDirectory, "ToWatch");
        if (!Directory.Exists(testDataFile))
        {
            _directoryToWatch = Directory.CreateDirectory(testDataFile);
        }
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<ISettingsService, SettingsService>();
        services.Configure<BatchTimerSettings>(options =>
        {
            options.IntervalMs = 1;
        });
        services.Configure<BatchSettings>(options =>
        {
            options.MaxItems = 400;
        });

        services.AddSingleton<IFileEventBatchProcessor, FileEventBatchProcessor>();
        services.AddSingleton<IFileEventDispatcher, FileEventDispatcher>();
        services.AddSingleton<IFileWatcherService, FileWatcherService>();
        services.AddSingleton<Interfaces.ITimer, ConcurrentTimer>();
        services.AddSingleton<ILoggingService, AppLogger>();

        _serviceProvider = services.BuildServiceProvider();
        _scope = _serviceProvider.CreateScope();

    }

    [TestMethod]
    public async Task e()
    {
        // Arrange
        int expectedFileCount = 20;
        var allEventsReceived = new TaskCompletionSource<List<FileSystemEventArgs>>();
        List<FileSystemEventArgs> fileEvents = [];
        var lockObject = new object();

        using var timeoutCts = new CancellationTokenSource(TimeSpan.FromMilliseconds(500));

        IFileWatcherService fileWatcher = _scope.ServiceProvider.GetRequiredService<IFileWatcherService>();

        fileWatcher.FileChanged += (sender, args) =>
        {
            lock (lockObject)
            {
                fileEvents.Add(args);
                if (fileEvents.Count >= expectedFileCount)
                {
                    allEventsReceived.TrySetResult(fileEvents);
                }
            }
        };

        // Act
        fileWatcher.StartWatcher(_directoryToWatch.FullName);
        IEnumerable<string> filePaths = CreateTextFiles(expectedFileCount);
        List<FileSystemEventArgs> receivedEvents = await allEventsReceived.Task.ConfigureAwait(false);

        // Assert
        receivedEvents.Select(x => x.FullPath).Should().BeEquivalentTo(filePaths);
    }

    private List<string> CreateTextFiles(int numberOfFiles)
    {
        List<string> filePaths = [];
        for (int i = 0; i < numberOfFiles; i++)
        {
            string fileName = $"testfile_{i}.txt";
            string filePath = Path.Combine(_directoryToWatch.FullName, fileName);
            File.WriteAllText(filePath, "This is a test file.");
            filePaths.Add(filePath);
        }
        return filePaths;
    }

    [TestCleanup]
    public void Cleanup()
    {
        _scope?.ServiceProvider.GetRequiredService<IFileWatcherService>().StopWatcher();
        _scope?.Dispose();
        _serviceProvider?.Dispose();
        if (_directoryToWatch != null && _directoryToWatch.Exists)
        {
            _directoryToWatch.Delete(true);
        }
    }
}
