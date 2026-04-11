using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.ViewModels;

public class GridSpaceDetailsViewModel
{
    public required GridSpace GridSpace;
    public required List<Tuple<GridCharacter, Character>> GridCharacters;
    public required List<Tuple<GridEncounter, Encounter>> GridEncounters;
    public required List<Encounter> Encounters;
    public required List<Character> Characters;
}