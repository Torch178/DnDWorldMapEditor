using System;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace DnDWorldMapEditor.HelperFunctions;

public static class FileFunctions
{
    public static async void ReplaceExistingImage(string oldImagePath, string newImagePath , IFormFile newImage)
    {
        try
        {
            File.Delete(oldImagePath);
            await using var fileStream = new FileStream(newImagePath, FileMode.Create);
            await newImage.CopyToAsync(fileStream);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        
    }
    
    public static string GenerateUniqueFileName(string file)
    {
        string ext = Path.GetExtension(file);
        string uniqueName = Guid.NewGuid().ToString();
        string uniqueFileName = uniqueName + ext;
        return uniqueFileName;
    }
}