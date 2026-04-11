namespace DnDWorldMapEditor.ViewModels;

public class GridSpaceEditDetailsViewModel
{
    public string? History { get; set; }
    public string? Description { get; set; }
    public string? Notes { get; set; }
    public required bool Accessible { get; set; }
    
}