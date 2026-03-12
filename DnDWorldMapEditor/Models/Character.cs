using System.ComponentModel.DataAnnotations;

namespace DnDWorldMapEditor.Models;

public class Character
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    [MaxLength(450)]
    public string? UserId { get; set; }
}