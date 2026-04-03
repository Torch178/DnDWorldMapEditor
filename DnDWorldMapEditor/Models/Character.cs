using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace DnDWorldMapEditor.Models;

public class Character
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    [MaxLength(450)]
    public string? UserId { get; set; }

    [SetsRequiredMembers]
    public Character(string name, string description, string userId)
    {
        Name = name;
        Description = description;
        UserId = userId;
    }
}