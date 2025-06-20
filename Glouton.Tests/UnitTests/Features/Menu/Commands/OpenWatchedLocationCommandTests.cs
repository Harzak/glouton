using FakeItEasy;
using FluentAssertions;
using Glouton.Features.Glouton;
using Glouton.Features.Menu.Commands;
using Glouton.Interfaces;
using Glouton.Settings;
using Glouton.ViewModels;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities.ObjectModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Tests.UnitTests.Features.Menu.Commands;

[TestClass]
public class OpenWatchedLocationCommandTests
{
    private ISettingsService _settingsService;
    private IProcessFacade _processFacade;
    private IDirectoryFacade _directoryFacade;

    [TestInitialize]
    public void Initialize()
    {
        _settingsService = A.Fake<ISettingsService>();
        _processFacade = A.Fake<IProcessFacade>();
        _directoryFacade = A.Fake<IDirectoryFacade>();
    }

    [TestMethod]
    public void Command_CannotBeExecuted_When_NoWatchedDirectory()
    {
        //Arrange
        OpenWatchedLocationCommand command = new(_settingsService, _processFacade, _directoryFacade);
        A.CallTo(() => _settingsService.GetSettings()).Returns(new AppSettings
        {
            WatchedFilePath = ""
        });

        //Act
        bool canBeExecuted = command.CanExecute();

        //Assert
        canBeExecuted.Should().BeFalse();
    }

    //testing the test here
    [TestMethod]
    public void Command_ShouldOpen_WatchedDirectory()
    {
        //Arrange
        OpenWatchedLocationCommand command = new(_settingsService, _processFacade, _directoryFacade);
        string watchedDirectory = "C:\\Temp";
        A.CallTo(() => _settingsService.GetSettings()).Returns(new AppSettings
        {
            WatchedFilePath = watchedDirectory
        });
        A.CallTo(() => _directoryFacade.Exists(watchedDirectory)).Returns(true);

        //Act
        command.Execute();

        //Assert
        A.CallTo(() => _processFacade.Start(A<ProcessStartInfo>._)).MustHaveHappenedOnceExactly();
    }
}
