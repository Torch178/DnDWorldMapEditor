using DnDWorldMapEditor.Data;
using Microsoft.EntityFrameworkCore;

namespace DnDWorldMapEditor.Tests.TestHelperFunctions;

public static class DependencyAccessorFunctions
{
    public static ApplicationDbContext GetDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryDatabase")
            .Options;

        var dbContext = new ApplicationDbContext(options);
        dbContext.Database.EnsureCreated();
        return new  ApplicationDbContext(options);
    }
}