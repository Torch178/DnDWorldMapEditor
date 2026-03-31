using Microsoft.AspNetCore.Http;

namespace DnDWorldMapEditor.ViewModels;

public class WorldMapEditViewModel
{
    public required string Name { get; set; } = "TestWorld";
    public string? Description { get; set; } =  "TestDescription";
    public required string OldImage = "OldImage";
    public IFormFile? NewImage { get; set; }
}