using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

public DbSet<DnDWorldMapEditor.Models.Encounter> Encounter { get; set; } = default!;

public DbSet<DnDWorldMapEditor.Models.Character> Character { get; set; } = default!;

public DbSet<DnDWorldMapEditor.Models.WorldMap> WorldMap { get; set; } = default!;

public DbSet<DnDWorldMapEditor.Models.GridSpace> GridSpace { get; set; } = default!;

public DbSet<DnDWorldMapEditor.Models.GridEncounter> GridEncounter { get; set; } = default!;

public DbSet<DnDWorldMapEditor.Models.GridCharacter> GridCharacter { get; set; } = default!;
}