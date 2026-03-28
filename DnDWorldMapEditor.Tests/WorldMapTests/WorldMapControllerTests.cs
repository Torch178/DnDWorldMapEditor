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

namespace DnDWorldMapEditor.Tests.WorldMapTests;


public class WorldMapControllerTests
{
    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        var dbContext = new ApplicationDbContext(options);
        dbContext.Database.EnsureCreated();
        return new  ApplicationDbContext(options);
    }
    
    
    
    private readonly WorldMapController _worldMapController;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<WorldMapController> _logger;

    public WorldMapControllerTests()
    {
        //Dependencies
        var environment = A.Fake<IWebHostEnvironment>();
        _context = GetDbContext();
        _worldMapController = new WorldMapController(_context, environment, _logger);
        
        //SUT

    }

    private async void SetupWorldMapController_Edit_Tests()
    {
        
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
        
        string MapSize = "Small";
        
        WorldMap wM = new WorldMap
        {
            UserId = Guid.NewGuid().ToString(),
            Name = "TestWorld",
            Description =  "TestWorld",
            MapSize = "Small",
            TotalRows = 5,
            TotalColumns = 5,
            BackgroundImage = "test_image.jpg"
        };
        
        _context.Add(wM);
        await _context.SaveChangesAsync();
        int worldMapId = wM.Id;
        for (int i = 0; i < wM.TotalRows; i++)
        {
            for (int j = 0; j < wM.TotalColumns; j++)
            {
                GridSpace gridSpace = new GridSpace(worldMapId,  i, j);
                _context.GridSpace.Add(gridSpace);
                await _context.SaveChangesAsync();
            }
        }
    }

    [Fact]
    public async Task WorldMapController_Index_ReturnsSuccess()
    {
        //Arrange
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
        
        //Act
        var result = _worldMapController.Index();

        //Assert
        result.Should().BeOfType<Task<IActionResult>>();

    }

    [Fact]
    public async Task WorldMapController_Create_ReturnsSuccess()
    {
        //Arrange
        await _context.Database.EnsureDeletedAsync();
        await _context.Database.EnsureCreatedAsync();
        var parentDir = Directory.GetParent(Directory.GetCurrentDirectory()).Name;
        var filePath = Path.Combine(parentDir,"TestResources","Images","test_image.jpg");
        IFormFile file = A.Fake<IFormFile>();
        string oldFileName = file.FileName;
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "TestWorld",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        //Act
        var results = await _worldMapController.Create(newMap);

        //Assert
        
    }

    [Fact]
    public async Task WorldMapController_Edit_ColumnNumIncrease_ReturnsUpdatedGridSpaces()
    {
        //Arrange
        SetupWorldMapController_Edit_Tests();
        WorldMap wM = await _context.WorldMap.FirstOrDefaultAsync() ?? throw new InvalidOperationException();
        List<GridSpace> oldGridSpaces = await _context.GridSpace.Where(x => x.WorldMapId == wM.Id).ToListAsync();
        int oldGridSpaceCount = oldGridSpaces.Count;
        List<int> oldGridSpacesIds =  oldGridSpaces.Select(x => x.Id).ToList();
        int oldMapData = wM.TotalColumns;

        //Act
        wM.TotalColumns += 1;
        var results = await _worldMapController.Edit(wM.Id, wM );
        
        List<GridSpace> newGridSpaces = await _context.GridSpace.Where(x => x.WorldMapId == wM.Id).ToListAsync();
        List<int> newGridSpaceIds = newGridSpaces.Select(x => x.Id).ToList();
        int expectedGridSpaceCount = wM.TotalColumns * wM.TotalRows;

        //Assert
        results.Should().BeOfType<RedirectToActionResult>();
        newGridSpaces.Count.Should().BeGreaterThan(oldGridSpaceCount);
        newGridSpaces.Count.Should().Be(expectedGridSpaceCount);
        oldGridSpacesIds.Should().BeSubsetOf(newGridSpaceIds);

        for (int i = 0; i < wM.TotalColumns; i++)
        {
            newGridSpaces.Select(x => x.Col == i).Count().Should().Be(wM.TotalRows);
        }
        for (int i = 0; i < wM.TotalRows; i++)
        {
            newGridSpaces.Select(x => x.Col == i).Count().Should().Be(wM.TotalColumns);
        }

        foreach (GridSpace gridSpace in newGridSpaces)
        {
            gridSpace.WorldMapId.Should().Be(wM.Id);
            gridSpace.Row.Should().BeInRange(0, wM.TotalRows - 1);
            gridSpace.Col.Should().BeInRange(0, wM.TotalColumns - 1);
        }
    }
    
}