using DnDWorldMapEditor.Controllers;
using DnDWorldMapEditor.Data;
using DnDWorldMapEditor.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DnDWorldMapEditor.Tests.WorldMapTests;


public class WorldMapControllerTests
{
    private async Task<ApplicationDbContext> GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        var dbContext = new ApplicationDbContext(options);
        dbContext.Database.EnsureCreated();
        if (await dbContext.WorldMap.CountAsync() < 0)
        {
            dbContext.WorldMap.Add(
                new WorldMap()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Name = "World Map",
                    Description = "Lorem Ipsum",
                    TotalRows = 5,
                    TotalColumns = 5,
                    BackgroundImage = "example.jpg"

                });
            await dbContext.SaveChangesAsync();
        }
        
        return new  ApplicationDbContext(options);
    }
    
    private IWebHostEnvironment _environment;
    private WorldMapController _worldMapController;

    public WorldMapControllerTests()
    {
        //Dependencies
        
        
        //SUT
        
    }

    [Fact]
    public async void WorldMapController_Index_ReturnsSuccess()
    {
        //Arrange
        var context = await GetDbContext();
        _environment =  A.Fake<IWebHostEnvironment>();
        _worldMapController = new WorldMapController(context, _environment);
        
        //Act
        var result = _worldMapController.Index();

        //Assert
        result.Should().BeOfType<Task<IActionResult>>();

    }
}