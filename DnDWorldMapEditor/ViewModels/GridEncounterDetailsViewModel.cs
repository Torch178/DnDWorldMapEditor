using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.ViewModels;

public class GridEncounterDetailsViewModel
{
    public required Dictionary<GridEncounter, Encounter> GridEncounterDetails;
    public required int GridSpaceId;
}