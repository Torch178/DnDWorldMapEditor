namespace DnDWorldMapEditor.ViewModels;

public class WorldMapCreateViewModel
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required string MapSize { get; set; }
    public required IFormFile BackgroundImage { get; set; }
    
}