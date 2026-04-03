using DnDWorldMapEditor.Controllers;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static DnDWorldMapEditor.Tests.Functions.GeneratorFunctions;
namespace DnDWorldMapEditor.Tests.WorldMapTests;

[Collection("WorldMapTests")]
public class WorldMapControllerDeleteGridSpacesTests
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
    public async Task GSC_DeleteGridSpace_DeletesGridSpace()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(2, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        var count = await context.GridSpace.CountAsync();
        count.Should().Be(2);

        GridSpace? gridSpaceToDelete = await context.GridSpace.FirstAsync();
        int deletedId = gridSpaceToDelete.Id;

        //Act
        await worldMapController.DeleteGridSpace(deletedId);
        count = await context.GridSpace.CountAsync();
        
        //Assert
        count.Should().Be(1);
        gridSpaceToDelete = await context.GridSpace.FindAsync(deletedId);
        gridSpaceToDelete.Should().BeNull();
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GSC_DeleteGridSpace_DeletesGridEncounters()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(2, 1);
        await context.AddRangeAsync(gridSpaces);
        
        foreach (var gridSpace in gridSpaces)
        {
            List<GridEncounter> gridEncounters = GenerateGridEncounters(2, 1, gridSpace.Id);
            await context.GridEncounter.AddRangeAsync(gridEncounters);
        }
        await context.SaveChangesAsync();

        var count = await context.GridSpace.CountAsync();
        count.Should().Be(2);

        count = await context.GridEncounter.CountAsync();
        count.Should().Be(4);

        GridSpace gridSpaceToDelete = await context.GridSpace.FirstAsync();
        int deletedId = gridSpaceToDelete.Id;

        //Act
        await worldMapController.DeleteGridSpace(deletedId);
        count = await context.GridEncounter.CountAsync();
        count.Should().Be(2);
        var deletedGridEncounters = context.GridEncounter.Where(x => x.GridSpaceId == deletedId);
        deletedGridEncounters.Count().Should().Be(0);
        
        //Assert
  

        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task GSC_DeleteGridSpace_DeletesGridCharacters()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(2, 1);
        await context.AddRangeAsync(gridSpaces);
        
        foreach (var gridSpace in gridSpaces)
        {
            List<GridCharacter> gridCharacters = GenerateGridCharacters(2, 1, gridSpace.Id);
            await context.GridCharacter.AddRangeAsync(gridCharacters);
        }
        await context.SaveChangesAsync();

        var count = await context.GridSpace.CountAsync();
        count.Should().Be(2);

        count = await context.GridCharacter.CountAsync();
        count.Should().Be(4);

        GridSpace gridSpaceToDelete = await context.GridSpace.FirstAsync();
        int deletedId = gridSpaceToDelete.Id;

        //Act
        await worldMapController.DeleteGridSpace(deletedId);
        count = await context.GridCharacter.CountAsync();
        var deletedGridCharacters = context.GridCharacter.Where(x => x.GridSpaceId == deletedId);
        
        //Assert
        count.Should().Be(2);
        deletedGridCharacters.Count().Should().Be(0);
        await context.Database.EnsureDeletedAsync();
    }
}