namespace DnDWorldMapEditor.Models;

public class GridCharacter
{
    public int Id { get; set; }
    public int GridSpaceId { get; set; }
    public int CharacterId { get; set; }

    public GridCharacter(int gridSpaceId, int characterId)
    {
        GridSpaceId = gridSpaceId;
        CharacterId = characterId;
    }
}