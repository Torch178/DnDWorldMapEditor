using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.ViewModels;

public class GridSpaceDetailsViewModel
{
    public required GridSpace gridSpace;
    public List<GridCharacter> gridCharacters;
    public List<GridEncounter> gridEncounters;
}