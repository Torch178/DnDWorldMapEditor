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
using Microsoft.Extensions.Logging;
using static DnDWorldMapEditor.Tests.TestHelperFunctions.TestSetupFunctions;
using static DnDWorldMapEditor.Tests.TestHelperFunctions.MockingFunctions;
using static DnDWorldMapEditor.Tests.TestHelperFunctions.DependencyAccessorFunctions;
namespace DnDWorldMapEditor.Tests.WorldMapTests;

public class WorldMapControllerCreateWorldMapTests
{
    //-----------------------------------------------
    //-----------------------------------------------
    #region Setup & Helper Functions
    
    
    private readonly WorldMapController _worldMapController;
    private readonly ApplicationDbContext _context;
    private readonly  IWebHostEnvironment _environment;
    public WorldMapControllerCreateWorldMapTests()
    {
        //Dependencies
        _environment = A.Fake<IWebHostEnvironment>();
        _context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        _worldMapController = new WorldMapController(_context, _environment, logger);
        
        //SUT

    }
    #endregion
    //-----------------------------------------------
    //-----------------------------------------------

    [Fact]
    public async Task WorldMapController_Create_ReturnsSuccess()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        //Act
        var result = _worldMapController.Create();

        //Assert
        result.Should().BeOfType<ViewResult>();
        await CleanupTests(_context);

    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_NameIsEmptyOrWhiteSpace_Returns_BadRequest()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("testImage", "image/png", "test.png");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        //Act
        var results = await _worldMapController.Create(newMap);

        //Assert
        results.Should().BeOfType<BadRequestObjectResult>();
        _context.WorldMap.Count().Should().Be(0);
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsBadFormat_Returns_BadRequest()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("testImage", "image/png", "test.png");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Big",
            BackgroundImage = file
        };

        //Act
        var results = await _worldMapController.Create(newMap);

        //Assert
        results.Should().BeOfType<BadRequestObjectResult>();
        _context.WorldMap.Count().Should().Be(0);
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsSmall_Returns_SmallWorldMap()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("TestImage", "image/png", "test.png");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);

        //Assert
        _context.WorldMap.First().MapSize.Should().Be("Small");
        _context.WorldMap.First().TotalRows.Should().Be(5);
        _context.WorldMap.First().TotalColumns.Should().Be(5);
        
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsMedium_Returns_MediumWorldMap()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("TestImage", "image/png", "test.png");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Medium",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);

        //Assert
        _context.WorldMap.First().MapSize.Should().Be("Medium");
        _context.WorldMap.First().TotalRows.Should().Be(7);
        _context.WorldMap.First().TotalColumns.Should().Be(7);
        
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsLarge_Returns_LargeWorldMap()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("TestImage", "image/png", "test.png");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);

        //Assert
        _context.WorldMap.First().MapSize.Should().Be("Large");
        _context.WorldMap.First().TotalRows.Should().Be(10);
        _context.WorldMap.First().TotalColumns.Should().Be(10);
        
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_BGImageIsEmpty_Returns_BadRequest()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("", "image/png", "test.png");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        var results = await _worldMapController.Create(newMap);

        //Assert
        Assert.IsType<BadRequestObjectResult>(results);
        _context.WorldMap.Count().Should().Be(0);
        
        await CleanupTests(_context);
    }

    [Fact]
    public async Task WMC_CreateWorldMap_BGImageIsPNG_Returns_PNG()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("PNG Test Image", "image/png", "test.png");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);
        WorldMap wm = _context.WorldMap.First();
        var imageFilePath = Path.Combine(_environment.WebRootPath, "images", "worldMaps", wm.BackgroundImage);
        
        //Assert
        _context.WorldMap.First().BackgroundImage.Should().EndWith(".png");
        File.Exists(imageFilePath).Should().BeTrue();
        
        await CleanupTests(_context);
    }

    [Fact]
    public async Task WMC_CreateWorldMap_BGImageIsJPEG_Returns_JPEG()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);
        WorldMap wm = _context.WorldMap.First();
        var imageFilePath = Path.Combine(_environment.WebRootPath, "images", "worldMaps", wm.BackgroundImage);
        
        //Assert
        _context.WorldMap.First().BackgroundImage.Should().EndWith(".jpeg");
        File.Exists(imageFilePath).Should().BeTrue();
        
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_ValidInput_Returns_MatchingName()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);
        WorldMap wm = _context.WorldMap.First();
        
        
        //Assert
        wm.Name.Should().Be(newMap.Name);
        
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_ValidInput_Returns_MatchingDescription()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);
        WorldMap wm = _context.WorldMap.First();

        
        //Assert
        wm.Description.Should().Be(newMap.Description);
        
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_ValidInput_Returns_MatchingMapSize()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);
        WorldMap wm = _context.WorldMap.First();

        
        //Assert
        wm.MapSize.Should().Be(newMap.MapSize);
        
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_ValidInput_Returns_CorrectUserId()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);
        WorldMap wm = _context.WorldMap.First();
        var userId = _worldMapController.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

        
        //Assert
        wm.UserId.Should().Be(userId);
        
        await CleanupTests(_context);
    }
    [Fact]
    public async Task WMC_CreateWorldMap_ValidBGImage_Returns_UniqueImageFileName()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);
        WorldMap wm = _context.WorldMap.First();
        _worldMapController.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        int nameLength = Guid.NewGuid().ToString().Length;
        int endIndex = wm.BackgroundImage.LastIndexOf('.');
        
        //Assert
        wm.BackgroundImage.Should().NotBe(file.FileName);
        wm.BackgroundImage.Substring(0, endIndex).Length.Should().Be(nameLength);
        
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsSmall_Returns_CorrectNumTotalGridSpaces()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);
        
        //Assert
         _context.GridSpace.Count().Should().Be(25);
        
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsMedium_Returns_CorrectNumTotalGridSpaces()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Medium",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);
        
        //Assert
        _context.GridSpace.Count().Should().Be(49);
        
        await CleanupTests(_context);
    }
    
    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsLarge_Returns_CorrectNumTotalGridSpaces()
    {
        //Arrange
        await SetupTests(_context);
        _worldMapController.ControllerContext = CreateMockUser();
        
        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await _worldMapController.Create(newMap);
        
        //Assert
        _context.GridSpace.Count().Should().Be(100);
        
        await CleanupTests(_context);
    }
}