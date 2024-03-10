using System;
using System.Collections.Generic;
using UnityEngine;

public class EndTile : Tile
{
    [SerializeField] private SpriteRenderer enabledSprite;
    [SerializeField] private SpriteRenderer enabledSpriteEdges;
    [SerializeField] private SpriteRenderer disabledSprite;
    private LockTile[] locks;
    private int LocksCount => locks.Length;
    private int openedLocks = 0;
    
    private void Awake()
    {
        locks = FindObjectsOfType<LockTile>();
        foreach (LockTile lockTile in locks)
        {
            lockTile.OnLockOpened += OnLockOpened;
            lockTile.OnLockClosed += OnLockClosed;
        }
    }

    private void Start()
    {
        if (openedLocks < LocksCount)
        {
            enabledSprite.enabled = false;
            enabledSpriteEdges.enabled = false;
            disabledSprite.enabled = true;
        }
    }

    public override bool TryWalkingOnTile(Direction direction)
    {
        if (openedLocks < LocksCount)
        {
            return true;
        }
        Debug.Log("GG you completed the level :p");
        GameManager.Instance.LoadNextScene();
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
        }
    }
    
    private void OnLockClosed()
    {
        openedLocks--;
        enabledSprite.enabled = false;
        enabledSpriteEdges.enabled = false;
        disabledSprite.enabled = true;
    }
}
