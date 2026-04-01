using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.ViewModels;

public class WorldMapDetailsViewModel
{
    public required WorldMap WorldMap { get; set; }
    public required List<GridSpace> GridSpaces { get; set; }
    public List<GridCharacter>? GridCharacters { get; set; }
    public List<GridEncounter>? GridEncounters { get; set; }
}