namespace DnDWorldMapEditor.Models;

public class GridSpace
{
    public int Id { get; set; }
    public int WorldMapId { get; set; }
    public int Row { get; set; }
    public int Col { get; set; }
    public string? History { get; set; }
    public string? Description { get; set; }
    public bool Accessible { get; set; } = true;
    public string? Notes { get; set; }

    public GridSpace(int worldMapId, int row, int col)
    {
        WorldMapId = worldMapId;
        Row = row;
        Col = col;
        History = string.Empty;
        Description = string.Empty;
        Accessible = true;
        Notes = string.Empty;
    }
}