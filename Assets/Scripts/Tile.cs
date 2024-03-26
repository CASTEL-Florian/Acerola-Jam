using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public abstract bool TryWalkingOnTile(Direction direction, bool isPlayer = false);
    
    public virtual void RegisterSpriteRenderer(SpriteRenderer spriteRenderer)
    {
    }
}
