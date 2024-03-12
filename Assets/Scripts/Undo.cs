using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Undo : MonoBehaviour
{
    private GridManager gridManager;
    private Stack<short[,]> gridArrays = new Stack<short[,]>();
    private Stack<int> gridIndices = new Stack<int>();
    
    private PlayerMovement player;
    private Stack<Vector3> playerPositions = new Stack<Vector3>();
    private Stack<float> playerRotations = new Stack<float>();
    
    private PushableTile[] pushableTiles;
    private Stack<Vector3>[] pushablePositions;

    private KeyTile[] keyTiles;
    private Stack<bool>[] keyStates;
    
    private LockTile[] lockTiles;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        player = FindObjectOfType<PlayerMovement>();
        pushableTiles = Utils.GetAllObjectsOnlyInScene<PushableTile>();
        pushablePositions = new Stack<Vector3>[pushableTiles.Length];
        for (int i = 0; i < pushableTiles.Length; i++)
        {
            pushablePositions[i] = new Stack<Vector3>();
        }
        keyTiles = Utils.GetAllObjectsOnlyInScene<KeyTile>();
        keyStates = new Stack<bool>[keyTiles.Length];
        for (int i = 0; i < keyTiles.Length; i++)
        {
            keyStates[i] = new Stack<bool>();
        }
        lockTiles = Utils.GetAllObjectsOnlyInScene<LockTile>();
        Record();
    }

    public void Record()
    {
        playerPositions.Push(player.transform.position);
        playerRotations.Push(player.transform.eulerAngles.z);
        
        for (int i = 0; i < pushableTiles.Length; i++)
        {
            pushablePositions[i].Push(pushableTiles[i].transform.position);
        }
        
        short[,] gridArray = new short[gridManager.gridArray.GetLength(0), gridManager.gridArray.GetLength(1)];
        for (int i = 0; i < gridManager.gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridManager.gridArray.GetLength(1); j++)
            {
                gridArray[i, j] = (short)gridManager.gridArray[i, j];
            }
        }
        gridArrays.Push(gridArray);
        gridIndices.Push(gridManager.currentGridIndex);
        
        for (int i = 0; i < keyTiles.Length; i++)
        {
            keyStates[i].Push(keyTiles[i].IsActivated);
        }
    }
    
    public void UndoLast()
    {
        if (playerPositions.Count == 0)
        {
            return;
        }
        Vector3 playerPosition = playerPositions.Pop();
        float playerRotation = playerRotations.Pop();
        player.transform.position = playerPosition;
        player.transform.eulerAngles = new Vector3(0, 0, playerRotation);
        player.UpdateDirection();
        
        for (int i = 0; i < pushableTiles.Length; i++)
        {
            Vector2 currentPushablePosition = pushableTiles[i].transform.position;
            Vector2 newPushablePosition = pushablePositions[i].Pop();
            pushableTiles[i].transform.position = newPushablePosition;
            if(Vector2.Distance(currentPushablePosition, newPushablePosition) > 0.1f)
            {
                Vector2Int currentPushablePositionInt = new Vector2Int(Mathf.RoundToInt(currentPushablePosition.x), Mathf.RoundToInt(currentPushablePosition.y));
                Vector2Int newPushablePositionInt = new Vector2Int(Mathf.RoundToInt(newPushablePosition.x), Mathf.RoundToInt(newPushablePosition.y));
                gridManager.MoveObjectImmediate(pushableTiles[i].gameObject, currentPushablePositionInt, newPushablePositionInt);
            }
        }
        
        short[,] gridArray = gridArrays.Pop();
        for (int i = 0; i < gridManager.gridArray.GetLength(0); i++)
        {
            for (int j = 0; j < gridManager.gridArray.GetLength(1); j++)
            {
                gridManager.gridArray[i, j] = gridArray[i, j];
            }
        }
        gridManager.currentGridIndex = gridIndices.Pop();
        gridManager.ResumeGrid();
        
        for (int i = 0; i < keyTiles.Length; i++)
        {
            bool isActivated = keyStates[i].Pop();
            int keyIndex = keyTiles[i].KeyIndex;
            foreach (LockTile lockTile in lockTiles)
            {
                if (lockTile.KeyIndex == keyIndex)
                {
                    if (isActivated)
                    {
                        lockTile.OpenLock();
                    }
                    else
                    {
                        lockTile.CloseLock();
                    }
                }
            }
            if (isActivated)
            {
                keyTiles[i].Activate(true);
            }
            else
            {
                keyTiles[i].Deactivate();
            }
        }
    }
}
