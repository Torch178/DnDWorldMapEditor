using System.ComponentModel.DataAnnotations;

namespace DnDWorldMapEditor.Models;

public class WorldMap
{
    public int Id { get; set; }
    [MaxLength(450)]
    public string? UserId { get; set; }
    [MaxLength(100)]
    public required string Name { get; set; } = string.Empty;
    [MaxLength(1000)]
    public string? Description { get; set; } = string.Empty;

    public required int TotalRows { get; set; } = 5;
    public required int TotalColumns { get; set; } = 5;
    [MaxLength(6)]
    public required string MapSize { get; set; } = "Small";
    [MaxLength(450)]
    public required string BackgroundImage { get; set; } =  string.Empty;


}