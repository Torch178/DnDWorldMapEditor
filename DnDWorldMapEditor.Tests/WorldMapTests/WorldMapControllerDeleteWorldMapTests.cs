using System.Security.Claims;
using System.Text;
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
public class WorldMapControllerDeleteWorldMapTests
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
    public async Task WMC_DeleteValidId_ReturnsView()
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
        var result = await worldMapController.DeleteConfirmed(wm.Id);

        //Assert
        result.Should().BeOfType<ViewResult>();
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_DeleteInvalidId_ReturnsNotFound()
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
        var result = await worldMapController.Delete(1);

        //Assert
        result.Should().BeOfType<NotFoundResult>();
        
        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task WMC_DeleteWorldMap_DeletesGridSpacesWithWorldMapIdFK()
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
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        WorldMapCreateViewModel newMap1 = new WorldMapCreateViewModel()
        {
            Name = "WorldMap1",
            Description = "TestWorld",
            MapSize = "Medium",
            BackgroundImage = file
        };
        WorldMapCreateViewModel newMap2 = new WorldMapCreateViewModel()
        {
            Name = "WorldMap2",
            Description = "TestWorld Description",
            MapSize = "Medium",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap1);
        await worldMapController.Create(newMap2);
        context.WorldMap.Count().Should().Be(2);
        context.GridSpace.Count().Should().Be(98);
        var worldMap = await context.WorldMap.FirstAsync();
        int worldMapId = worldMap.Id;
        var result = await worldMapController.DeleteConfirmed(worldMapId);
        var deletedWorldMap = await context.WorldMap.FindAsync(worldMapId);
        var deletedGridSpaces = await context.GridSpace.Where(x => x.WorldMapId == worldMapId).ToListAsync();

        //Assert
        result.Should().BeOfType<Task<IActionResult>>();
        deletedWorldMap.Should().BeNull();
        deletedGridSpaces.Should().BeNull();
        context.WorldMap.Count().Should().Be(1);
        context.GridSpace.Count().Should().Be(49);
        
        
        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task WMC_DeleteWorldMap_ReturnsImageFileDeleted()
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
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        WorldMapCreateViewModel newMap1 = new WorldMapCreateViewModel()
        {
            Name = "WorldMap1",
            Description = "TestWorld",
            MapSize = "Medium",
            BackgroundImage = file
        };
        WorldMapCreateViewModel newMap2 = new WorldMapCreateViewModel()
        {
            Name = "WorldMap2",
            Description = "TestWorld Description",
            MapSize = "Medium",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap1);
        await worldMapController.Create(newMap2);
        var worldMap = await context.WorldMap.FirstAsync();
        int worldMapId = worldMap.Id;
        var imageFilePath = Path.Combine(environment.WebRootPath, "images", "worldMaps", worldMap.BackgroundImage);
        File.Exists(imageFilePath).Should().BeTrue();
        
        var result = await worldMapController.DeleteConfirmed(worldMapId);
        var deletedWorldMap = await context.WorldMap.FindAsync(worldMapId);

        //Assert
        result.Should().BeOfType<Task<IActionResult>>();
        deletedWorldMap.Should().BeNull();
        File.Exists(imageFilePath).Should().BeFalse();

        await context.Database.EnsureDeletedAsync();
    }
    
}