using System.ComponentModel.DataAnnotations;

namespace DnDWorldMapEditor.Models;

public class WorldMap
{
    public int Id { get; set; }
    [MaxLength(450)]
    public string? UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required int TotalRows { get; set; }
    public required int TotalColumns { get; set; }
    public required string BackgroundImage { get; set; }
}