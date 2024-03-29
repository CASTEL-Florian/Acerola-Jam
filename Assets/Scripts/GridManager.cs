using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    
    private PlayerMovement player;
    [SerializeField] private List<Grid> grids = new List<Grid>();
    [SerializeField] List<Vector2Int> gridIndices = new List<Vector2Int>();
    
    [HideInInspector] public int currentGridIndex = 0;
    
    private int minX = Int32.MaxValue;
    private int maxX = -Int32.MaxValue;
    private int minY = Int32.MaxValue;
    private int maxY = -Int32.MaxValue;
    
    public int[,] gridArray;
    
    public int GridCount => grids.Count;

    private Compass compass;
    
    
    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        compass = FindObjectOfType<Compass>();
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

        UpdateBehindObjects();

    }

    public void UpdateBehindObjects(bool hide = false)
    {
        if (hide) HideBehind(true);
        Vector2Int gridsBefore = gridIndices[(gridIndices.Count + currentGridIndex - 1) % gridIndices.Count];
        Vector2Int gridsAfter = gridIndices[(currentGridIndex + 1) % gridIndices.Count];
        foreach (GameObject go in grids[gridsBefore.x]
                     .GetAllObjects(true))
        {
            go.SetActive(true);
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            if (gridsBefore.x == gridsAfter.y)
            {
                sr.maskInteraction = SpriteMaskInteraction.None;
            }
        }

        if (gridsBefore.x != gridsAfter.y)
        {
            foreach (GameObject go in grids[gridsAfter.y]
                         .GetAllObjects(true))
            {
                go.SetActive(true);
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
        }
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
        HideBehind();
        StartCoroutine(UpdateBehindGridCoroutine(turn, playerPosition));
        if (turn == PlayerMovement.Turn.Left)
        {
            currentGridIndex = currentGridIndex == gridIndices.Count - 1 ? 0 : currentGridIndex + 1;
        }
        else
        {
            currentGridIndex = currentGridIndex == 0 ? gridIndices.Count - 1 : currentGridIndex - 1;
        }
        compass.PulseCurrentDirections(turn, gridIndices[currentGridIndex]);
        
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

    private void HideBehind(bool hideAll = false)
    {
        if (hideAll)
        {
            for (int i = 0; i < grids.Count; i++)
            {
                foreach (GameObject go in grids[i].GetAllObjects(true))
                {
                    go.SetActive(false);
                }
            }
        }
        Vector2Int gridsBefore = gridIndices[(gridIndices.Count + currentGridIndex - 1) % gridIndices.Count];
        Vector2Int gridsAfter = gridIndices[(currentGridIndex + 1) % gridIndices.Count];

        foreach (GameObject go in grids[gridsBefore.x]
                     .GetAllObjects(true))
        {
            go.SetActive(false);
        }
        if (gridsBefore.x != gridsAfter.y)
        {
            foreach (GameObject go in grids[gridsAfter.y]
                         .GetAllObjects(true))
            {
                go.SetActive(false);
            }
        }
    }
    
    private IEnumerator UpdateBehindGridCoroutine(PlayerMovement.Turn turn, Vector2Int playerPosition)
    {
        Vector2Int gridsBeforeBefore = gridIndices[(gridIndices.Count + currentGridIndex - 2) % gridIndices.Count];
        Vector2Int gridsBefore = gridIndices[(gridIndices.Count + currentGridIndex - 1) % gridIndices.Count];
        Vector2Int gridsAfter = gridIndices[(currentGridIndex + 1) % gridIndices.Count];
        Vector2Int gridsAfterAfter = gridIndices[(currentGridIndex + 2) % gridIndices.Count];
        
        int[] currentGridIndices = new int[]
        {
            gridsBeforeBefore.x,
            gridsBefore.x,
            gridsBefore.y,
            
            gridsAfter.x,
            gridsAfter.y,
            gridsAfterAfter.y,
        };

        int delta = turn == PlayerMovement.Turn.Left ? 1 : -1;
        Func<int, int> getIndex = i => delta * i > 0 ? currentGridIndices[2 + delta * i] : currentGridIndices[3 + delta * i];
        
        Direction direction = player.CurrentDirection;
        
        
        
        foreach (GameObject go in grids[getIndex(-2)]
                     .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 3), true))
        {
            go.SetActive(true);
            if (turn == PlayerMovement.Turn.Left)
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            if (getIndex(-2) == getIndex(3))
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            }
        }

        if (getIndex(-2) != getIndex(3))
        {
            foreach (GameObject go in grids[getIndex(3)]
                         .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 3), true))
            {
                go.SetActive(true);
                if (turn == PlayerMovement.Turn.Left)
                {
                    go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
                else
                {
                    go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
            }
        }
        foreach (GameObject go in grids[getIndex(2)]
                     .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 2), true))
        {
            go.SetActive(true);
            if (turn == PlayerMovement.Turn.Left)
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            else
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            if (getIndex(2) == getIndex(3))
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            }
        }
        if (getIndex(2) != getIndex(3))
        {
            foreach (GameObject go in grids[getIndex(3)]
                         .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 2), true))
            {
                go.SetActive(true);
                if (turn == PlayerMovement.Turn.Left)
                {
                    go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
                else
                {
                    go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
            }
        }
        foreach (GameObject go in grids[getIndex(2)]
                     .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 1), true))
        {
            go.SetActive(true);
            if (turn == PlayerMovement.Turn.Left)
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            else
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
        }
        foreach (GameObject go in grids[getIndex(3)]
                     .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 4), true))
        {
            go.SetActive(true);
            if (turn == PlayerMovement.Turn.Left)
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            else
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            if (getIndex(-1) == getIndex(3))
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
            }
        }

        if (getIndex(-1) != getIndex(3))
        {
            foreach (GameObject go in grids[getIndex(-1)]
                         .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 4), true))
            {
                go.SetActive(true);
                if (turn == PlayerMovement.Turn.Left)
                {
                    go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
                else
                {
                    go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
            }
        }
        foreach (GameObject go in grids[getIndex(-1)]
                     .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 5), true))
        {
            go.SetActive(true);
            if (turn == PlayerMovement.Turn.Left)
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                go.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            
        }
        
        
        yield return new WaitUntil(() => !player.IsMoving);


        foreach (GameObject go in grids[getIndex(-2)]
                     .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 4), true))
        {
            go.SetActive(false);
        }

        foreach (GameObject go in grids[getIndex(2)]
                     .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 1), true))
        {
            go.SetActive(false);
        }
        foreach (GameObject go in grids[getIndex(2)]
                     .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 2), true))
        {
            go.SetActive(false);
        }

        if (getIndex(-2) != getIndex(3))
        {
            foreach (GameObject go in grids[getIndex(-2)]
                         .GetObjectsInDirection(playerPosition, TurnDirection(direction, turn, 3), true))
            {
                go.SetActive(false);
            }
        }
        foreach (GameObject go in grids[getIndex(-1)]
                     .GetAllObjects(true))
        {
            go.SetActive(true);
            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
            if (turn == PlayerMovement.Turn.Left)
            {
                sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
            }
            else
            {
                sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            if (getIndex(3) == getIndex(-1))
            {
                sr.maskInteraction = SpriteMaskInteraction.None;
            }
        }

        if (getIndex(3) != getIndex(-1))
        {
            foreach (GameObject go in grids[getIndex(3)]
                         .GetAllObjects(true))
            {
                go.SetActive(true);
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if (turn == PlayerMovement.Turn.Left)
                {
                    sr.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                }
                else
                {
                    sr.maskInteraction = SpriteMaskInteraction.VisibleOutsideMask;
                }
            }
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

        UpdateCompass();
    }

    public void UpdateCompass(bool updateAll = false)
    {
        compass.UpdateColors(gridIndices[(gridIndices.Count + currentGridIndex - 1) % gridIndices.Count], gridIndices[currentGridIndex],
            gridIndices[(currentGridIndex + 1) % gridIndices.Count], updateAll);
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

    public void MoveObjectNextFrame(GameObject obj, Vector2Int startPosition, Vector2Int endPosition)
    {
        // Move the object at the next frame, otherwise the collection will be modified while iterating and foreach loops will throw an exception
        StartCoroutine(MoveObjectCoroutine(obj, startPosition, endPosition));
    }
    
    public void MoveObjectImmediate(GameObject obj, Vector2Int startPosition, Vector2Int endPosition)
    {
        MoveObject(obj, startPosition, endPosition);
    }

    public IEnumerator MoveObjectCoroutine(GameObject obj, Vector2Int startPosition, Vector2Int endPosition)
    {
        yield return null;
        MoveObject(obj, startPosition, endPosition);
    }

    private void MoveObject(GameObject obj, Vector2Int startPosition, Vector2Int endPosition)
    {
        PushableTile pushableTile = obj.GetComponent<PushableTile>();
        int startGridIndex = gridArray[startPosition.x - minX, startPosition.y - minY];
        grids[startGridIndex].RemoveObject(obj, startPosition);
        if (pushableTile != null)
        {
            grids[startGridIndex].RemoveObject(pushableTile.OtherTransformToUpdate.gameObject, startPosition, true);
        }
        int endGridIndex = gridArray[endPosition.x - minX, endPosition.y - minY];
        if (endGridIndex == -1)
        {
            grids[startGridIndex].AddObject(obj, endPosition);
            if (pushableTile != null)
            {
                grids[startGridIndex].AddObject(pushableTile.OtherTransformToUpdate.gameObject, endPosition, true);
            }
            gridArray[endPosition.x - minX, endPosition.y - minY] = startGridIndex;
            return;
        }
        grids[endGridIndex].AddObject(obj, endPosition);
        if (pushableTile != null)
        {
            grids[endGridIndex].AddObject(pushableTile.OtherTransformToUpdate.gameObject, endPosition, true);
        }
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
