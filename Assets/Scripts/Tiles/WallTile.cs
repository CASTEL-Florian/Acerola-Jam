public class WallTile : Tile
{
    public override bool TryWalkingOnTile(Direction direction)
    {
        return false;
    }
}
