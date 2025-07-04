﻿using FakeItEasy;
using FluentAssertions;
using Glouton.EventArgs;
using Glouton.Interfaces;
using Glouton.ViewModels;

namespace Glouton.Tests.UnitTests.ViewModels;

[TestClass]
public class GloutonViewModelTests : BaseViewModel
{
    private IGlouton _glouton;

    [TestInitialize]
    public void Initialize()
    {
        _glouton = A.Fake<IGlouton>();
    }

    [TestCleanup]
    public void Cleanup()
    {
        _glouton?.Dispose();
    }

    [DataRow(null)]
    [DataRow(-11256)]
    [DataRow(55698)]
    [DataRow(0)]
    [DataRow(40)]
    [TestMethod()]
    public void ImageId_ShouldBe_Between_0and4(int level)
    {
        //Arrange
        using GloutonViewModel viewModel = new(_glouton);

        //Act
        _glouton.HungerLevelChanged += Raise.FreeForm<EventHandler<HungerLevelEventArgs>>.With(_glouton, new HungerLevelEventArgs(level));

        //Assert
        viewModel.ImageId.Should().BeInRange(0, 4);
    }

    [TestMethod()]
    public void IncreasingHunger_ShouldChange_Image()
    {
        //Arrange
        using GloutonViewModel viewModel = new(_glouton);
        int previousImageId = viewModel.ImageId;

        //Act
        _glouton.HungerLevelChanged += Raise.FreeForm<EventHandler<HungerLevelEventArgs>>.With(_glouton, new HungerLevelEventArgs(70));

        //Assert
        viewModel.ImageId.Should().BeGreaterThan(previousImageId);
        viewModel.ImageId.Should().BeGreaterThan(GloutonViewModel.DefaultImageId);
    }

    [TestMethod()]
    public void ReducingHunger_ShouldChange_Image()
    {
        //Arrange
        using GloutonViewModel viewModel = new(_glouton);
        int previousImageId = viewModel.ImageId;

        //Act
        _glouton.HungerLevelChanged += Raise.FreeForm<EventHandler<HungerLevelEventArgs>>.With(_glouton, new HungerLevelEventArgs(10));

        //Assert
        viewModel.ImageId.Should().BeLessThan(previousImageId);
        viewModel.ImageId.Should().BeLessThan(GloutonViewModel.DefaultImageId);
    }

    [TestMethod]
    public void Constructor_ShouldInitialize_WithDefaultImageId()
    {
        //Arrange & Act
        using GloutonViewModel viewModel = new(_glouton);

        //Assert
        viewModel.ImageId.Should().Be(GloutonViewModel.DefaultImageId);
    }

    [DataRow(0, 0)]   
    [DataRow(25, 1)]  
    [DataRow(50, 2)]  
    [DataRow(75, 3)]  
    [DataRow(100, 4)] 
    [TestMethod]
    public void HungerLevel_ShouldMap_ToCorrectImageId(int hungerLevel, int expectedImageId)
    {
        //Arrange
        using GloutonViewModel viewModel = new(_glouton);

        //Act
        _glouton.HungerLevelChanged += Raise.FreeForm<EventHandler<HungerLevelEventArgs>>
            .With(_glouton, new HungerLevelEventArgs(hungerLevel));

        //Assert
        viewModel.ImageId.Should().Be(expectedImageId);
    }
}
