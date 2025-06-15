using FakeItEasy;
using Glouton.Features.FileManagement.FileEvent;
using Glouton.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glouton.Tests.Features.FileManagement.FileEvent;

[TestClass]
public class FileEventDispatcherTests
{
    private ILoggingService _loggingService;
    private ISettingsService _settingsService;

    [TestInitialize]
    public void Initialize()
    {
        _loggingService = A.Fake<ILoggingService>();    
        _settingsService = A.Fake<ISettingsService>();
    }

    [TestMethod]
    public void e()
    {
        //Arrange
        var e = new FileEventDispatcher(_loggingService, _settingsService);
        e.

        //Act

        //Assert
    }
}

