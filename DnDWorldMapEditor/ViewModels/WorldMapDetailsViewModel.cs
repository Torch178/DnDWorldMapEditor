using DnDWorldMapEditor.Models;

namespace DnDWorldMapEditor.ViewModels;

public class WorldMapDetailsViewModel
{
    public required WorldMap WorldMap;
    public required Dictionary<(int row, int col), string> Status;
}