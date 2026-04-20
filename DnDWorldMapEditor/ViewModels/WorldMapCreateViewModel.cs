using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DnDWorldMapEditor.ViewModels;



public class WorldMapCreateViewModel
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }
    
    public string? Description { get; set; }
    [Required]
    public required string MapSize { get; set; }
    [Required]
    public required IFormFile BackgroundImage { get; set; }
    
}