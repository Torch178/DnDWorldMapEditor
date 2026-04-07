using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.ViewModels;

public class AddEncounterToGridForm
{
    public required List<Encounter> Encounters;
    public required Encounter SelectedEncounter;
    public required int GridSpaceId;
}