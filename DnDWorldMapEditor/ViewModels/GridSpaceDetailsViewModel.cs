using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.ViewModels;

public class GridSpaceDetailsViewModel
{
    public required GridSpace GridSpace;
    public required List<GridCharacter> GridCharacters;
    public required List<GridEncounter> GridEncounters;
    public required List<Encounter> Encounters;
    public required List<Character> Characters;
}