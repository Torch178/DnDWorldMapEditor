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
public class WorldMapControllerDeleteWorldMapTest
{
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
        var result = await worldMapController.Delete(wm.Id);

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
    public async Task WMC_DeleteWorldMap_ReturnsDeletedGridSpaceData()
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
        WorldMapCreateViewModel newMap = new WorldMapCreateViewModel()
        {
            Name = "Name",
            Description = "TestWorld Description",
            MapSize = "Medium",
            BackgroundImage = file
        };

        //Act
        var result = await worldMapController.Create(newMap);
        

        //Assert
        result.Should().BeOfType<NotFoundResult>();
        
        await context.Database.EnsureDeletedAsync();
    }
    
}