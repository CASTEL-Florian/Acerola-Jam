using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    
    private PlayerMovement player;
    [SerializeField] private List<Grid> grids = new List<Grid>();
    [SerializeField] List<Vector2Int> gridIndices = new List<Vector2Int>();
    
    private int currentGridIndex = 0;
    
    private int minX = Int32.MaxValue;
    private int maxX = -Int32.MaxValue;
    private int minY = Int32.MaxValue;
    private int maxY = -Int32.MaxValue;
    
    private int[,] gridArray;
    
    public int GridCount => grids.Count;
    
    
    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        player.OnTurn += UpdateWorldRotation;
        player.OnFroward += UpdateWorldForward;
        
        for (int i = 0; i < grids.Count; i++)
        {
            grids[i].gameObject.SetActive(true);
            grids[i].transform.position = Vector3.zero;
            grids[i].Init();
            int gridMinX = grids[i].BottomLeft.x;
            int gridMaxX = gridMinX + grids[i].Width - 1;
            int gridMinY = grids[i].BottomLeft.y;
            int gridMaxY = gridMinY + grids[i].Height - 1;
            
            if (gridMinX < minX)
            {
                minX = gridMinX;
            }
            if (gridMaxX > maxX)
            {
                maxX = gridMaxX;
            }
            if (gridMinY < minY)
            {
                minY = gridMinY;
            }
            if (gridMaxY > maxY)
            {
                maxY = gridMaxY;
            }
        }
        
        gridArray = new int[maxX - minX + 1, maxY - minY + 1];
        for (int i = 0; i < maxX - minX + 1; i++)
        {
            for (int j = 0; j < maxY - minY + 1; j++)
            {
                gridArray[i, j] = -1;
            }
        }
        
        
        Vector2Int playerPosition = new Vector2Int(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        
        Vector2Int currentGrids = gridIndices[currentGridIndex];

        List<GameObject> startGameObjects =
            grids[currentGrids.x].GetObjectsInDirection(playerPosition, Direction.DownRight);
        foreach (GameObject go in grids[currentGrids.x]
                     .GetObjectsInDirection(playerPosition, Direction.UpRight))
        {
            startGameObjects.Add(go);
        }
        foreach (GameObject go in grids[currentGrids.x]
                     .GetObjectsInDirection(playerPosition, Direction.Right))
        {
            startGameObjects.Add(go);
        }
        foreach (GameObject go in grids[currentGrids.x]
                     .GetObjectsInDirection(playerPosition, Direction.Up))
        {
            startGameObjects.Add(go);
        }
        foreach (GameObject go in grids[currentGrids.x]
                     .GetObjectsInDirection(playerPosition + new Vector2Int(0, 1), Direction.Down))
        {
            startGameObjects.Add(go);
        }
        


        ShowObjects(startGameObjects, currentGrids.x);

    }


    private void ShowObjects(List<GameObject> objects, int gridIndex = -1)
    {
        foreach (GameObject go in objects)
        {
            Vector3 position = go.transform.position;
            if (gridIndex != -1)
            {
                gridArray[Mathf.RoundToInt(position.x) - minX, Mathf.RoundToInt(position.y) - minY] = gridIndex;
            }
            go.SetActive(true);
        }
    }
    
    private void HideObjects(List<GameObject> objects, bool removeFromGrid)
    {
        foreach (GameObject go in objects)
        {
            Vector3 position = go.transform.position;
            if (removeFromGrid)
            {
                gridArray[Mathf.RoundToInt(position.x) - minX, Mathf.RoundToInt(position.y) - minY] = -1;
            }

            go.SetActive(false);
        }
    }
    
    private void UpdateWorldRotation(PlayerMovement.Turn turn)
    {
        Vector2Int playerPosition = new Vector2Int(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        
        Direction playerDirection = player.CurrentDirection;
        Direction newDirection1 = TurnDirection(playerDirection, turn);
        Direction newDirection2 = TurnDirection(playerDirection, turn, 2);
        
        PlayerMovement.Turn oppositeTurn = turn == PlayerMovement.Turn.Left ? PlayerMovement.Turn.Right : PlayerMovement.Turn.Left;
        Direction oldDirection1 = TurnDirection(playerDirection, oppositeTurn, 3);
        Direction oldDirection2 = TurnDirection(playerDirection, oppositeTurn, 4);

        StartCoroutine(HideCoroutine(playerPosition, playerPosition, oldDirection1, oldDirection2));

        if (turn == PlayerMovement.Turn.Left)
        {
            currentGridIndex = currentGridIndex == gridIndices.Count - 1 ? 0 : currentGridIndex + 1;
        }
        else
        {
            currentGridIndex = currentGridIndex == 0 ? gridIndices.Count - 1 : currentGridIndex - 1;
        }
        
        Vector2Int currentGrids = gridIndices[currentGridIndex];
        
        if (turn == PlayerMovement.Turn.Left)
        {
            ShowObjects(grids[currentGrids.y].GetObjectsInDirection(playerPosition, newDirection1), currentGrids.y);
            ShowObjects(grids[currentGrids.y].GetObjectsInDirection(playerPosition, newDirection2), currentGrids.y);
        }
        else
        {
            ShowObjects(grids[currentGrids.x].GetObjectsInDirection(playerPosition, newDirection1), currentGrids.x);
            ShowObjects(grids[currentGrids.x].GetObjectsInDirection(playerPosition, newDirection2), currentGrids.x);
        }
    }

    private IEnumerator HideCoroutine(Vector2Int playerPosition1, Vector2Int playerPosition2, Direction oldDirection1, Direction oldDirection2)
    {
        yield return new WaitUntil(() => !player.IsMoving);
        foreach (var grid in grids)
        {
            HideObjects(grid.GetObjectsInDirection(playerPosition1, oldDirection1), true);
            HideObjects(grid.GetObjectsInDirection(playerPosition2, oldDirection2), true);
        }
        
    }
    
    private Direction TurnDirection(Direction direction, PlayerMovement.Turn turn, int turnCount = 1)
    {
        int sign = turn == PlayerMovement.Turn.Left ? 1 : -1;
        int directionInt = ((int)direction + turnCount * sign) % 8;
        if (directionInt < 0)
        {
            directionInt += 8;
        }
        
        return (Direction)directionInt;
    }

    private void UpdateWorldForward(Vector2Int oldPosition)
    {
        switch (player.CurrentDirection)
        {
            case Direction.Up:
            case Direction.Down:
                StartCoroutine(HideCoroutine(oldPosition,oldPosition + new Vector2Int(-1, 0), Direction.Left, Direction.Right));

                break;
            case Direction.Left:
            case Direction.Right:
                StartCoroutine(HideCoroutine(oldPosition,oldPosition + new Vector2Int(0, 1), Direction.Up, Direction.Down));

                break;
        }
    }

    public void MoveObject(GameObject obj, Vector2Int startPosition, Vector2Int endPosition)
    {
        StartCoroutine(MoveObjectCoroutine(obj, startPosition, endPosition));
    }

    public IEnumerator MoveObjectCoroutine(GameObject obj, Vector2Int startPosition, Vector2Int endPosition)
    {
        yield return null;
        int startGridIndex = gridArray[startPosition.x - minX, startPosition.y - minY];
        grids[startGridIndex].RemoveObject(obj, startPosition);
        int endGridIndex = gridArray[endPosition.x - minX, endPosition.y - minY];
        if (endGridIndex == -1)
        {
            grids[startGridIndex].AddObject(obj, endPosition);
            gridArray[endPosition.x - minX, endPosition.y - minY] = startGridIndex;
            yield break;
        }
        grids[endGridIndex].AddObject(obj, endPosition);
    }
    
    public List<GameObject> GetObjectsAtPosition(Vector2Int position)
    {
        List<GameObject> objects = new List<GameObject>();
        if (position.x < minX || position.x > maxX || position.y < minY || position.y > maxY)
        {
            return objects;
        }
        int gridIndex = gridArray[position.x - minX, position.y - minY];
        if (gridIndex == -1)
        {
            return objects;
        }
        Vector2Int gridPosition = new Vector2Int(position.x - grids[gridIndex].BottomLeft.x, position.y - grids[gridIndex].BottomLeft.y);
        return grids[gridIndex].GetObjectsAtGridPosition(gridPosition);
    }

    public void ShowGrid(int index)
    {
        foreach (var grid in grids)
        {
            HideObjects(grid.GetObjectsInDirection(new Vector2Int(minX-1, minY-1), Direction.UpRight), false);
        }
        ShowObjects(grids[index].GetObjectsInDirection(new Vector2Int(minX-1, minY-1), Direction.UpRight), -1);
    }

    public void ResumeGrid()
    {
        foreach (var grid in grids)
        {
            HideObjects(grid.GetObjectsInDirection(new Vector2Int(minX - 1, minY - 1), Direction.UpRight), false);
        }

        for (int x = minX; x < minX + gridArray.GetLength(0); x++)
        {
            for (int y = minY; y < minY + gridArray.GetLength(1); y++)
            {
                ShowObjects(GetObjectsAtPosition(new Vector2Int(x, y)));
            }
        }
    }

}
