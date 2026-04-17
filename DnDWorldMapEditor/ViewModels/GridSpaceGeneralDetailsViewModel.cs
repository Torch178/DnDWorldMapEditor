using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.ViewModels;

public class GridSpaceGeneralDetailsViewModel
{
    public required GridSpace GridSpace;
    public required int GridCharacterCount;
    public required int GridEncounterCount;
    
    public GridSpaceGeneralDetailsViewModel(){

    }
    
    public GridSpaceGeneralDetailsViewModel(GridSpace gridSpace, int gridCharacterCount, int gridEncounterCount)
    {
        GridSpace = gridSpace;
        GridCharacterCount = gridCharacterCount;
        GridEncounterCount = gridEncounterCount;

    }
}

