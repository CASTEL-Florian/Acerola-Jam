using UnityEngine;

public class EndTile : Tile
{
    public override bool TryWalkingOnTile(Direction direction)
    {
        Debug.Log("GG you completed the level :p");
        GameManager.Instance.LoadNextScene();
        return true;
    }
}
