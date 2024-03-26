public class WallTile : Tile
{
    public override bool TryWalkingOnTile(Direction direction, bool isPlayer = false)
    {
        return false;
    }
}
