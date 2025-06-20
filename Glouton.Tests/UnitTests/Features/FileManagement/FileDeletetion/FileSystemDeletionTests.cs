using FakeItEasy;
using FluentAssertions;
using Glouton.Features.FileManagement.FileDeletion;
using Glouton.Interfaces;
using Glouton.Utils.Result;

namespace Glouton.Tests.UnitTests.Features.FileManagement.FileDeletetion;

[TestClass]
public class FileSystemDeletionTests
{
    private const int MAX_ATTEMPTS = 5;  

    private IFileSystemFacade _fileDeletionProxy;
    private IDirectoryFacade _directoryDeletionProxy;
    private IRetryPolicy _retryPolicy;

    private FileSystemDeletion _fileSystemDeletion;

    [TestInitialize]
    public void Initialize()
    {
        _fileDeletionProxy = A.Fake<IFileSystemFacade>();
        _directoryDeletionProxy = A.Fake<IDirectoryFacade>();
        _retryPolicy = A.Fake<IRetryPolicy>();

        A.CallTo(() => _retryPolicy.GetNextRetryDelay(A<int>.Ignored)).Returns(TimeSpan.Zero);
        A.CallTo(() => _retryPolicy.MaxAttemps).Returns(MAX_ATTEMPTS);

        _fileSystemDeletion = new FileSystemDeletion(_fileDeletionProxy, _directoryDeletionProxy, _retryPolicy);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("  ")]
    public async Task Deletion_ShouldFailed_WithEmptyPath(string path)
    {
        // Arrange
        A.CallTo(() => _fileDeletionProxy.Exists(path)).Returns(false);
        A.CallTo(() => _directoryDeletionProxy.Exists(path)).Returns(false);

        // Act
        OperationResult result = await _fileSystemDeletion.StartAsync(path).ConfigureAwait(false);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    [DataRow("F:\\Directory")]
    [DataRow("F:\\file.txt")]
    public async Task Deletion_ShouldFailed_WithNontExistentFileOrDirectory(string path)
    {
        //Arrange
        A.CallTo(() => _fileDeletionProxy.Exists(path)).Returns(false);
        A.CallTo(() => _directoryDeletionProxy.Exists(path)).Returns(false);

        //Act
        OperationResult result = await _fileSystemDeletion.StartAsync(path).ConfigureAwait(false);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
        A.CallTo(() => _fileDeletionProxy.Delete(path)).MustNotHaveHappened();
        A.CallTo(() => _directoryDeletionProxy.Delete(path)).MustNotHaveHappened();
    }

    [TestMethod]
    [DataRow("F:\\file.txt")]
    public async Task Deletion_ShouldRetry_WhenFileDeletionFailed(string path)
    {
        //Arrange
        A.CallTo(() => _fileDeletionProxy.Exists(path)).Returns(true);

        //Act
        OperationResult result = await _fileSystemDeletion.StartAsync(path).ConfigureAwait(false);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
        A.CallTo(() => _fileDeletionProxy.Delete(path)).MustHaveHappened(MAX_ATTEMPTS, Times.Exactly);
    }

    [TestMethod]
    [DataRow("F:\\Directory")]
    public async Task Deletion_ShouldRetry_WhenDirectoryDeletionFailed(string path)
    {
        //Arrange
        A.CallTo(() => _directoryDeletionProxy.Exists(path)).Returns(true);

        //Act
        OperationResult result = await _fileSystemDeletion.StartAsync(path).ConfigureAwait(false);

        //Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
        A.CallTo(() => _directoryDeletionProxy.Delete(path)).MustHaveHappened(MAX_ATTEMPTS, Times.Exactly);
    }

    [TestMethod]
    [DataRow("F:\\file.txt")]
    public async Task Deletion_ShouldDelete_File(string path)
    {
        //Arrange
        A.CallTo(() => _fileDeletionProxy.Exists(path)).ReturnsNextFromSequence(true, false);

        //Act
        OperationResult result = await _fileSystemDeletion.StartAsync(path).ConfigureAwait(false);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNullOrEmpty();
        A.CallTo(() => _fileDeletionProxy.Delete(path)).MustHaveHappenedOnceExactly();
    }

    [TestMethod]
    [DataRow("F:\\Directory")]
    public async Task Deletion_ShouldDelete_NesteadDirectoryFiles(string path)
    {
        //Arrange
        string nestedFile2 = Path.Combine(path, "nestedFile2.txt");
        string nestedFile1 = Path.Combine(path, "nestedFile1.txt"); 
        A.CallTo(() => _directoryDeletionProxy.Exists(path)).ReturnsNextFromSequence(true, false);
        A.CallTo(() => _directoryDeletionProxy.GetFiles(path)).Returns([nestedFile1, nestedFile2]);

        //Act
        OperationResult result = await _fileSystemDeletion.StartAsync(path).ConfigureAwait(false);

        //Assert
        result.IsSuccess.Should().BeTrue();
        result.ErrorMessage.Should().BeNullOrEmpty();
        A.CallTo(() => _fileDeletionProxy.Delete(nestedFile1)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _fileDeletionProxy.Delete(nestedFile2)).MustHaveHappenedOnceExactly();
        A.CallTo(() => _directoryDeletionProxy.Delete(path)).MustHaveHappenedOnceExactly();
    }
}