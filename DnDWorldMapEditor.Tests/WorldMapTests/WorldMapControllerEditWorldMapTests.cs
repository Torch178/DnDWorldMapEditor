using DnDWorldMapEditor.Controllers;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;
using DnDWorldMapEditor.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static DnDWorldMapEditor.Tests.Functions.GeneratorFunctions;

namespace DnDWorldMapEditor.Tests.WorldMapTests;

[Collection("WorldMapTests")]
public class WorldMapControllerEditWorldMapTests
{

    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        var dbContext = new ApplicationDbContext(options);
        dbContext.Database.EnsureCreated();
        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task WMC_EditValidId_ReturnsView()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("testImage", "image/png", "test.png");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Test",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        await worldMapController.Create(newMap);
        context.WorldMap.Count().Should().Be(1);
        WorldMap wm = context.WorldMap.First();


        //Act
        var result = await worldMapController.Edit(wm.Id);

        //Assert
        result.Should().BeOfType<ViewResult>();
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_EditInvalidId_ReturnsNotFound()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();
        context.WorldMap.Count().Should().Be(0);

        //Act
        var result = await worldMapController.Edit(1);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_Edit_UpdateName_ReturnsUpdatedName()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("testImage", "image/png", "test.png");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Test",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        await worldMapController.Create(newMap);
        context.WorldMap.Count().Should().Be(1);
        WorldMap wm = await context.WorldMap.FirstAsync();
        wm.Name.Should().Be(newMap.Name);

        WorldMapEditViewModel updateMap = new WorldMapEditViewModel
        {
            Name = "New and Improved Name",
            Description = wm.Description,
            OldImage = wm.BackgroundImage,
            NewImage = null
        };


        //Act
        await worldMapController.Edit(wm.Id, updateMap);


        //Assert
        wm.Name.Should().Be(updateMap.Name);
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_Edit_UpdateDescription_ReturnsUpdatedDescription()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("testImage", "image/png", "test.png");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Test",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        await worldMapController.Create(newMap);
        context.WorldMap.Count().Should().Be(1);
        WorldMap wm = await context.WorldMap.FirstAsync();
        wm.Description.Should().Be(newMap.Description);

        WorldMapEditViewModel updateMap = new WorldMapEditViewModel
        {
            Name = wm.Name,
            Description = "New and Improved Description",
            OldImage = wm.BackgroundImage,
            NewImage = null
        };

        //Act
        await worldMapController.Edit(wm.Id, updateMap);

        //Assert
        wm.Description.Should().Be(updateMap.Description);
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_Edit_UpdateBGImage_ReturnsUpdatedImage()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("testImage", "image/png", "test.png");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Test",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        await worldMapController.Create(newMap);
        context.WorldMap.Count().Should().Be(1);
        WorldMap wm = await context.WorldMap.FirstAsync();
        wm.Description.Should().Be(newMap.Description);

        var newImage = CreateMockFile("testImage", "image/png", "test.png");

        WorldMapEditViewModel updateMap = new WorldMapEditViewModel
        {
            Name = wm.Name,
            Description = wm.Description,
            OldImage = wm.BackgroundImage,
            NewImage = newImage
        };


        //Act
        await worldMapController.Edit(wm.Id, updateMap);


        //Assert
        var newImagePath = Path.Combine(environment.WebRootPath, "images", "worldMaps", wm.BackgroundImage);
        var oldImagePath = Path.Combine(environment.WebRootPath, "images", "worldMaps", updateMap.OldImage);
        File.Exists(oldImagePath).Should().BeFalse();
        File.Exists(newImagePath).Should().BeTrue();

        
        await context.Database.EnsureDeletedAsync();
    }
}