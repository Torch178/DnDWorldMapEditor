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

namespace DnDWorldMapEditor.Tests.WorldMapTests;

[Collection("WorldMapTests")]
public class WorldMapControllerCreateWorldMapTests
{
    //-----------------------------------------------
    //-----------------------------------------------

    #region Setup & Helper Functions

    private ControllerContext CreateMockUser()
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "testUser")
        };
        var identity = new ClaimsIdentity(claims, "TestAuthentication");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        ControllerContext newContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        return newContext;
    }

    private IFormFile CreateMockFile(string fileContent, string contentType, string fileName)
    {
        var content = Encoding.UTF8.GetBytes(fileContent);
        IFormFile file = new FormFile(new MemoryStream(content), 0, content.Length, "test", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = contentType
        };

        return file;
    }

    private ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        var dbContext = new ApplicationDbContext(options);
        dbContext.Database.EnsureCreated();
        return new ApplicationDbContext(options);
    }

    #endregion

    //-----------------------------------------------
    //-----------------------------------------------

    [Fact]
    public async Task WorldMapController_Create_ReturnsSuccess()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        worldMapController.ControllerContext = CreateMockUser();

        //Act
        var result = worldMapController.Create();

        //Assert
        result.Should().BeOfType<ViewResult>();
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_NameIsEmptyOrWhiteSpace_Returns_BadRequest()
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
            Name = "",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        //Act
        var results = await worldMapController.Create(newMap);

        //Assert
        results.Should().BeOfType<BadRequestObjectResult>();
        context.WorldMap.Count().Should().Be(0);
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsBadFormat_Returns_BadRequest()
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
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Big",
            BackgroundImage = file
        };

        //Act
        var results = await worldMapController.Create(newMap);

        //Assert
        results.Should().BeOfType<BadRequestObjectResult>();
        context.WorldMap.Count().Should().Be(0);
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsSmall_Returns_SmallWorldMap()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("TestImage", "image/png", "test.png");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);

        //Assert
        context.WorldMap.First().MapSize.Should().Be("Small");
        context.WorldMap.First().TotalRows.Should().Be(5);
        context.WorldMap.First().TotalColumns.Should().Be(5);

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsMedium_Returns_MediumWorldMap()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("TestImage", "image/png", "test.png");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Medium",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);

        //Assert
        context.WorldMap.First().MapSize.Should().Be("Medium");
        context.WorldMap.First().TotalRows.Should().Be(7);
        context.WorldMap.First().TotalColumns.Should().Be(7);
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsLarge_Returns_LargeWorldMap()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("TestImage", "image/png", "test.png");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);

        //Assert
        context.WorldMap.First().MapSize.Should().Be("Large");
        context.WorldMap.First().TotalRows.Should().Be(10);
        context.WorldMap.First().TotalColumns.Should().Be(10);

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_BGImageIsEmpty_Returns_BadRequest()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("", "image/png", "test.png");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        var results = await worldMapController.Create(newMap);

        //Assert
        Assert.IsType<BadRequestObjectResult>(results);
        context.WorldMap.Count().Should().Be(0);

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_BGImageIsPNG_Returns_PNG()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("PNG Test Image", "image/png", "test.png");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);
        WorldMap wm = context.WorldMap.First();
        var imageFilePath = Path.Combine(environment.WebRootPath, "images", "worldMaps", wm.BackgroundImage);

        //Assert
        context.WorldMap.First().BackgroundImage.Should().EndWith(".png");
        File.Exists(imageFilePath).Should().BeTrue();

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_BGImageIsJPEG_Returns_JPEG()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);
        WorldMap wm = await context.WorldMap.FirstAsync();
        var imageFilePath = Path.Combine(environment.WebRootPath, "images", "worldMaps", wm.BackgroundImage);

        //Assert
        wm.BackgroundImage.Should().EndWith(".jpeg");
        File.Exists(imageFilePath).Should().BeTrue();
        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_ValidInput_Returns_MatchingName()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);
        WorldMap wm = await context.WorldMap.FirstAsync();


        //Assert
        wm.Name.Should().Be(newMap.Name);

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_ValidInput_Returns_MatchingDescription()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);
        WorldMap wm = context.WorldMap.First();


        //Assert
        wm.Description.Should().Be(newMap.Description);

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_ValidInput_Returns_MatchingMapSize()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);
        WorldMap wm = context.WorldMap.First();


        //Assert
        wm.MapSize.Should().Be(newMap.MapSize);

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_ValidInput_Returns_CorrectUserId()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);
        WorldMap wm = context.WorldMap.First();
        var userId = worldMapController.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);


        //Assert
        wm.UserId.Should().Be(userId);

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_ValidBGImage_Returns_UniqueImageFileName()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);
        WorldMap wm = await context.WorldMap.FirstAsync();
        worldMapController.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        int nameLength = Guid.NewGuid().ToString().Length;
        int endIndex = wm.BackgroundImage.LastIndexOf('.');

        //Assert
        wm.BackgroundImage.Should().NotBe(file.FileName);
        wm.BackgroundImage.Substring(0, endIndex).Length.Should().Be(nameLength);

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsSmall_Returns_CorrectNumTotalGridSpaces()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Small",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);

        //Assert
        context.GridSpace.Count().Should().Be(25);

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsMedium_Returns_CorrectNumTotalGridSpaces()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Medium",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);

        //Assert
        context.GridSpace.Count().Should().Be(49);

        
        await context.Database.EnsureDeletedAsync();
    }

    [Fact]
    public async Task WMC_CreateWorldMap_MapSizeIsLarge_Returns_CorrectNumTotalGridSpaces()
    {
        //Arrange
        var environment = A.Fake<IWebHostEnvironment>();
        var context = GetDbContext();
        var logger = A.Fake<ILogger<WorldMapController>>();
        var worldMapController = new WorldMapController(context, environment, logger);

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
        worldMapController.ControllerContext = CreateMockUser();

        IFormFile file = CreateMockFile("JPEG Test Image", "image/jpeg", "test.jpeg");

        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Large",
            BackgroundImage = file
        };

        //Act
        await worldMapController.Create(newMap);

        //Assert
        context.GridSpace.Count().Should().Be(100);
        
        await context.Database.EnsureDeletedAsync();
        worldMapController.Dispose();
        
    }
}