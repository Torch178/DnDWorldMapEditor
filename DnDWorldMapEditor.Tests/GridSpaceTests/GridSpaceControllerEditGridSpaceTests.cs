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
    public async Task GSC_EditGridSpaceValidId_ReturnsPartialView()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var gridSpaceController = new GridSpaceController(context, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        GridSpaceEditDetailsViewModel model = new GridSpaceEditDetailsViewModel()
        {
            Description = "Test Description",
            History = "Some History",
            Notes = "Test Notes",
            Accessible = true
        };
        

        //Act
        var results = await gridSpaceController.Edit(id, model);
        
        //Assert
        results.Should().BeOfType<PartialViewResult>();
        
        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task GSC_EditGridSpaceInvalidId_ReturnsObjectNotFound()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var gridSpaceController = new GridSpaceController(context, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        GridSpaceEditDetailsViewModel model = new GridSpaceEditDetailsViewModel()
        {
            Description = "Test Description",
            History = "Some History",
            Notes = "Test Notes",
            Accessible = true
        };
        

        //Act
        var results = await gridSpaceController.Edit(id, model);
        
        //Assert
        results.Should().BeOfType<NotFoundResult>();
        
        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task GSC_EditGridSpace_UpdatesGridSpaceHistory()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var gridSpaceController = new GridSpaceController(context, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        GridSpaceEditDetailsViewModel model = new GridSpaceEditDetailsViewModel()
        {
            Description = "Test Description",
            History = "Some History",
            Notes = "Test Notes",
            Accessible = true
        };
        

        //Act
        await gridSpaceController.Edit(id, model);
        
        //Assert
        var updatedGridSpace = await context.GridSpace.FindAsync(id);
        updatedGridSpace.Should().NotBeNull();
        updatedGridSpace.History.Should().Be(model.History);
        
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GSC_EditGridSpace_UpdatesGridSpaceDescription()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var gridSpaceController = new GridSpaceController(context, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        GridSpaceEditDetailsViewModel model = new GridSpaceEditDetailsViewModel()
        {
            Description = "Test Description",
            History = "Some History",
            Notes = "Test Notes",
            Accessible = true
        };
        

        //Act
        await gridSpaceController.Edit(id, model);
        
        //Assert
        var updatedGridSpace = await context.GridSpace.FindAsync(id);
        updatedGridSpace.Should().NotBeNull();
        updatedGridSpace.Description.Should().Be(model.Description);
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task GSC_EditGridSpace_UpdatesGridSpaceNotes()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var gridSpaceController = new GridSpaceController(context, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        GridSpaceEditDetailsViewModel model = new GridSpaceEditDetailsViewModel()
        {
            Description = "Test Description",
            History = "Some History",
            Notes = "Test Notes",
            Accessible = true
        };
        

        //Act
        await gridSpaceController.Edit(id, model);
        
        //Assert
        var updatedGridSpace = await context.GridSpace.FindAsync(id);
        updatedGridSpace.Should().NotBeNull();
        updatedGridSpace.Notes.Should().Be(model.Notes);
        
        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task GSC_EditGridSpace_UpdatesGridSpaceAccessibility()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var gridSpaceController = new GridSpaceController(context, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        gridSpaceController.ControllerContext = CreateMockUser();


        List<GridSpace> gridSpaces = GenerateGridSpaces(1, 1);
        await context.AddRangeAsync(gridSpaces);
        await context.SaveChangesAsync();

        GridSpace? gridSpace = await context.GridSpace.FirstOrDefaultAsync();
        gridSpace.Should().NotBeNull();
        int id = gridSpace.Id;

        GridSpaceEditDetailsViewModel model = new GridSpaceEditDetailsViewModel()
        {
            Description = "Test Description",
            History = "Some History",
            Notes = "Test Notes",
            Accessible = true
        };
        

        //Act
        await gridSpaceController.Edit(id, model);
        
        //Assert
        var updatedGridSpace = await context.GridSpace.FindAsync(id);
        updatedGridSpace.Should().NotBeNull();
        updatedGridSpace.Accessible.Should().Be(model.Accessible);
        
        await context.Database.EnsureDeletedAsync();
    }
    
    [Fact]
    public async Task GSC_AddCharacterToGridSpace_CreatesNewGridCharacter()
    {
        //Arrange
        var context = GetDbContext();
        var logger = A.Fake<ILogger<GridSpaceController>>();
        var gridSpaceController = new GridSpaceController(context, logger);

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
        

        //Act
        await gridSpaceController.AddCharacterToGridSpace(id, character);
        
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
        var gridSpaceController = new GridSpaceController(context, logger);

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
        

        //Act
        await gridSpaceController.AddCharacterToGridSpace(id, character);
        
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
        var gridSpaceController = new GridSpaceController(context, logger);

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
        

        //Act
        await gridSpaceController.AddCharacterToGridSpace(id, character);
        
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
        var gridSpaceController = new GridSpaceController(context, logger);

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
        

        //Act
        await gridSpaceController.AddEncounterToGridSpace(id, encounter);
        
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
        var gridSpaceController = new GridSpaceController(context, logger);

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
        

        //Act
        await gridSpaceController.AddEncounterToGridSpace(id, encounter);
        
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
        var gridSpaceController = new GridSpaceController(context, logger);

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
        

        //Act
        await gridSpaceController.AddEncounterToGridSpace(id, encounter);
        
        //Assert
        var gridEncounter = await context.GridEncounter.FirstOrDefaultAsync();
        gridEncounter.Should().NotBeNull();
        gridEncounter.EncounterId.Should().Be(encounter.Id);
        
        await context.Database.EnsureDeletedAsync();
    }
}