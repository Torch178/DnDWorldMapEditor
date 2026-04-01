namespace DnDWorldMapEditor.Models;

public class GridEncounter
{
    public int Id { get; set; }
    public int GridSpaceId { get; set; }
    public int EncounterId { get; set; }
    public bool IsCompleted { get; set; }
}