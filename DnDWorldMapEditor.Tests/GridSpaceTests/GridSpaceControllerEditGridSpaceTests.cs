using DnDWorldMapEditor.Controllers;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;
using DnDWorldMapEditor.ViewModels;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using static DnDWorldMapEditor.Tests.Functions.GeneratorFunctions;

namespace DnDWorldMapEditor.Tests.GridSpaceTests;

[Collection("WorldMapTests")]
public class GridSpaceControllerEditGridSpaceTests
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
    public async Task GSC_EditGridSpace_UpdatesGridSpaceHistory()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var environment = A.Fake<IWebHostEnvironment>();
        var gridSpaceController = new GridSpaceController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;
        string newHistory = "updated history";
        

        //Act
        await gridSpaceController.UpdateHistory(id, newHistory);
        
        //Assert
        var updatedGridSpace = await context.GridSpace.FindAsync(id);
        updatedGridSpace.Should().NotBeNull();
        updatedGridSpace.History.Should().Be(newHistory);
        
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GSC_EditGridSpace_UpdatesGridSpaceDescription()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var environment = A.Fake<IWebHostEnvironment>();
        var gridSpaceController = new GridSpaceController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;
        string newDescription = "Updated Description";
        

        //Act
        await gridSpaceController.UpdateDescription(id, newDescription);
        
        //Assert
        var updatedGridSpace = await context.GridSpace.FindAsync(id);
        updatedGridSpace.Should().NotBeNull();
        updatedGridSpace.Description.Should().Be(newDescription);
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GSC_EditGridSpace_UpdatesGridSpaceNotes()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var environment = A.Fake<IWebHostEnvironment>();
        var gridSpaceController = new GridSpaceController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;
        string newNotes = "Updated Notes";
        

        //Act
        await gridSpaceController.UpdateNotes(id, newNotes);
        
        //Assert
        var updatedGridSpace = await context.GridSpace.FindAsync(id);
        updatedGridSpace.Should().NotBeNull();
        updatedGridSpace.Notes.Should().Be(newNotes);
        
        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task GSC_EditGridSpace_UpdatesGridSpaceAccessibility()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var environment = A.Fake<IWebHostEnvironment>();
        var gridSpaceController = new GridSpaceController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;
        bool accessibility = gridSpace.Accessible;
        

        //Act
        await gridSpaceController.UpdateAccessibility(id);
        
        //Assert
        var updatedGridSpace = await context.GridSpace.FindAsync(id);
        updatedGridSpace.Should().NotBeNull();
        updatedGridSpace.Accessible.Should().Be(!accessibility);
        
        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task GSC_AddCharacterToGridSpace_CreatesNewGridCharacter()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var environment = A.Fake<IWebHostEnvironment>();
        var gridSpaceController = new GridSpaceController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        Character character = GenerateCharacters(1, "User1").First();
        await context.Character.AddAsync(character);
        await context.SaveChangesAsync();
        

        //Act
        await gridSpaceController.AddCharacterToGridSpace(id, character.Id);
        
        //Assert
        var count = await context.GridCharacter.CountAsync();
        count.Should().Be(1);
        
        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task GSC_AddCharacterToGridSpace_NewGridCharacterMatchesGridSpaceId()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var environment = A.Fake<IWebHostEnvironment>();
        var gridSpaceController = new GridSpaceController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        Character character = GenerateCharacters(1, "User1").First();
        await context.Character.AddAsync(character);
        await context.SaveChangesAsync();

        //Act
        await gridSpaceController.AddCharacterToGridSpace(id, character.Id);
        
        //Assert
        var gridCharacter = await context.GridCharacter.FirstOrDefaultAsync();
        gridCharacter.Should().NotBeNull();
        gridCharacter.GridSpaceId.Should().Be(id);
        
        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task GSC_AddCharacterToGridSpace_NewGridCharacterMatchesCharacterId()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var environment = A.Fake<IWebHostEnvironment>();
        var gridSpaceController = new GridSpaceController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        Character character = GenerateCharacters(1, "User1").First();
        await context.Character.AddAsync(character);
        await context.SaveChangesAsync();

        //Act
        await gridSpaceController.AddCharacterToGridSpace(id, character.Id);
        
        //Assert
        var gridCharacter = await context.GridCharacter.FirstOrDefaultAsync();
        gridCharacter.Should().NotBeNull();
        gridCharacter.CharacterId.Should().Be(character.Id);
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GSC_AddEncounterToGridSpace_CreatesNewGridEncounter()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var environment = A.Fake<IWebHostEnvironment>();
        var gridSpaceController = new GridSpaceController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        Encounter encounter = GenerateEncounters(1, "User1").First();
        await context.Encounter.AddAsync(encounter);
        await context.SaveChangesAsync();

        //Act
        await gridSpaceController.AddEncounterToGridSpace(id, encounter.Id);
        
        //Assert
        var count = await context.GridEncounter.CountAsync();
        count.Should().Be(1);
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GSC_AddEncounterToGridSpace_NewGridEncounterMatchesGridSpaceId()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var environment = A.Fake<IWebHostEnvironment>();
        var gridSpaceController = new GridSpaceController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        Encounter encounter = GenerateEncounters(1, "User1").First();
        await context.Encounter.AddAsync(encounter);
        await context.SaveChangesAsync();

        //Act
        await gridSpaceController.AddEncounterToGridSpace(id, encounter.Id);
        
        //Assert
        var gridEncounter = await context.GridEncounter.FirstOrDefaultAsync();
        gridEncounter.Should().NotBeNull();
        gridEncounter.GridSpaceId.Should().Be(id);
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GSC_AddEncounterToGridSpace_NewGridEncounterMatchesEncounterId()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var environment = A.Fake<IWebHostEnvironment>();
        var gridSpaceController = new GridSpaceController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        Encounter encounter = GenerateEncounters(1, "User1").First();
        await context.Encounter.AddAsync(encounter);
        await context.SaveChangesAsync();

        //Act
        await gridSpaceController.AddEncounterToGridSpace(id, encounter.Id);
        
        //Assert
        var gridEncounter = await context.GridEncounter.FirstOrDefaultAsync();
        gridEncounter.Should().NotBeNull();
        gridEncounter.EncounterId.Should().Be(encounter.Id);
        
        await context.Database.EnsureDeletedAsync();
    }
    
    
}