using System;
using System.Collections.Generic;
using UnityEngine;

public class EndTile : Tile
{
    [SerializeField] private SpriteRenderer enabledSprite;
    [SerializeField] private SpriteRenderer enabledSpriteEdges;
    [SerializeField] private SpriteRenderer disabledSprite;
    private LockTile[] locks;
    
    private SpriteRenderer otherSpriteRenderer;
    private int LocksCount => locks.Length;
    private int openedLocks = 0;
    
    private void Start()
    {
        locks = Utils.GetAllObjectsOnlyInScene<LockTile>();
        foreach (LockTile lockTile in locks)
        {
            lockTile.OnLockOpened += OnLockOpened;
            lockTile.OnLockClosed += OnLockClosed;
        }
        
        if (openedLocks < LocksCount)
        {
            enabledSprite.enabled = false;
            enabledSpriteEdges.enabled = false;
            disabledSprite.enabled = true;
            
            if (otherSpriteRenderer != null)
            {
                otherSpriteRenderer.sprite = disabledSprite.sprite;
                otherSpriteRenderer.color = disabledSprite.color;
            }
        }
        else
        {
            if (otherSpriteRenderer != null)
            {
                otherSpriteRenderer.sprite = enabledSprite.sprite;
                otherSpriteRenderer.color = enabledSprite.color;
            }
        }
    }

    public override bool TryWalkingOnTile(Direction direction, bool isPlayer = false)
    {
        if (openedLocks < LocksCount || !isPlayer)
        {
            return true;
        }
        Debug.Log("GG you completed the level :p");
        GameManager.Instance.Win();
        return true;
    }
    
    private void OnLockOpened()
    {
        openedLocks++;
        if (openedLocks == LocksCount)
        {
            enabledSprite.enabled = true;
            enabledSpriteEdges.enabled = true;
            disabledSprite.enabled = false;
            if (otherSpriteRenderer != null)
            {
                otherSpriteRenderer.sprite = enabledSprite.sprite;
                otherSpriteRenderer.color = enabledSprite.color;
            }
        }
    }
    
    private void OnLockClosed()
    {
        openedLocks--;
        enabledSprite.enabled = false;
        enabledSpriteEdges.enabled = false;
        disabledSprite.enabled = true;
        if (otherSpriteRenderer != null)
        {
            otherSpriteRenderer.sprite = disabledSprite.sprite;
            otherSpriteRenderer.color = disabledSprite.color;
        }
    }
    
    public override void RegisterSpriteRenderer(SpriteRenderer spriteRenderer)
    {

        spriteRenderer.sprite = enabledSprite.sprite;
        spriteRenderer.color = enabledSprite.color;

        if (locks != null && openedLocks < LocksCount)
        {
            spriteRenderer.sprite = disabledSprite.sprite;
            spriteRenderer.color = disabledSprite.color;
        }

        spriteRenderer.sortingOrder = enabledSprite.sortingOrder - 16;
        otherSpriteRenderer = spriteRenderer;
    }
}
