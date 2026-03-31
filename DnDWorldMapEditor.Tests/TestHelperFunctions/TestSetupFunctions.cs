using DnDWorldMapEditor.Data;

namespace DnDWorldMapEditor.Tests.TestHelperFunctions;

public static class TestSetupFunctions
{
    public static async Task SetupTests(ApplicationDbContext context)
    {
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    public static async Task CleanupTests(ApplicationDbContext context)
    {
        var rootPath = AppContext.BaseDirectory;
        var wwwrootPath = Path.Combine(rootPath, "wwwroot");
        string imageDirPath = Path.Combine(wwwrootPath,"images", "worldMaps");

        if (Directory.Exists(imageDirPath))
        { 
            string[] files = Directory.GetFiles(imageDirPath);
            try
            {
                foreach (string file in files)
                {
                    File.Delete(file);
                }

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        await context.Database.EnsureDeletedAsync();
    }
}