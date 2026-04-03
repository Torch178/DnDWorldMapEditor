namespace DnDWorldMapEditor.Models;

public class GridEncounter
{
    public int Id { get; set; }
    public int GridSpaceId { get; set; }
    public int EncounterId { get; set; }
    public bool IsCompleted { get; set; }

    public GridEncounter(int gridSpaceId, int encounterId, bool isCompleted)
    {
        GridSpaceId = gridSpaceId;
        EncounterId = encounterId;
        IsCompleted = isCompleted;
    }
}