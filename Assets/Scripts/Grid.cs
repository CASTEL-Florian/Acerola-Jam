using System;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Right,
    UpRight,
    Up,
    UpLeft,
    Left,
    DownLeft,
    Down,
    DownRight
}

public class Grid : MonoBehaviour
{
    private List<GameObject>[,] gridArray;
    private List<GameObject>[,] gridArray2;
    
    private List<GameObject> allObjects = new List<GameObject>();
    private List<GameObject> allObjects2 = new List<GameObject>();
    
    public int Width => gridArray.GetLength(0);
    public int Height => gridArray.GetLength(1);

    public Vector2Int BottomLeft { get; private set; }
    public void Init()
    {
        int minX = Int32.MaxValue;
        int maxX = -Int32.MaxValue;
        int minY = Int32.MaxValue;
        int maxY = -Int32.MaxValue;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
            Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.y));
            if (gridPosition.x < minX)
            {
                minX = gridPosition.x;
            }
            if (gridPosition.x > maxX)
            {
                maxX = gridPosition.x;
            }
            if (gridPosition.y < minY)
            {
                minY = gridPosition.y;
            }
            if (gridPosition.y > maxY)
            {
                maxY = gridPosition.y;
            }
        }
        gridArray = new List<GameObject>[maxX - minX + 1, maxY - minY + 1];
        gridArray2 = new List<GameObject>[maxX - minX + 1, maxY - minY + 1];
        BottomLeft = new Vector2Int(minX, minY);
        foreach (Transform child in transform)
        {
            Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(child.position.x), Mathf.RoundToInt(child.position.y));
            gridArray[gridPosition.x - minX, gridPosition.y - minY] ??= new List<GameObject>();
            gridArray[gridPosition.x - minX, gridPosition.y - minY].Add(child.gameObject);
            allObjects.Add(child.gameObject);
            
            gridArray2[gridPosition.x - minX, gridPosition.y - minY] ??= new List<GameObject>();
            GameObject obj = new GameObject();
            obj.transform.position = child.position;
            SpriteRenderer rend = obj.AddComponent<SpriteRenderer>();
            SpriteRenderer childRend = child.GetComponent<SpriteRenderer>();
            if (childRend != null)
            {
                rend.sprite = childRend.sprite;
                rend.sortingOrder = childRend.sortingOrder - 16;
                rend.color = childRend.color;
            }
            else
            {
                Tile tile = child.GetComponent<Tile>();
                tile.RegisterSpriteRenderer(rend);
            }
            
            PushableTile pushableTile = child.GetComponent<PushableTile>();
            if (pushableTile != null)
            {
                pushableTile.RegisterOtherTransformToUpdate(obj.transform);
            }
            obj.layer = LayerMask.NameToLayer("Behind");
            
            gridArray2[gridPosition.x - minX, gridPosition.y - minY].Add(obj);
            allObjects2.Add(obj);
            obj.SetActive(false);
        }
    }

    public List<GameObject> GetObjectsAtGridPosition(Vector2Int position, bool secondaryArray = false)
    {
        if (position.x < 0 || position.x >= Width || position.y < 0 || position.y >= Height || gridArray[position.x, position.y] == null)
        {
            return new List<GameObject>();
        }
        if (secondaryArray)
        {
            return gridArray2[position.x, position.y];
        }
        return gridArray[position.x, position.y];
    }

    public List<GameObject> GetObjectsInDirection(Vector2Int worldPosition, Direction direction, bool secondaryArray = false)
    {
        List<GameObject> objects = new List<GameObject>();
        Vector2Int position = new Vector2Int(worldPosition.x - BottomLeft.x, worldPosition.y - BottomLeft.y);
        switch (direction)
        {
            case Direction.Up:
                for (int y = position.y + 1; y < Height; y++)
                {
                    var temp = GetObjectsAtGridPosition(new Vector2Int(position.x, y), secondaryArray);
                    for (int i = 0; i < temp.Count; i++)
                    {
                        objects.Add(temp[i]);
                    }
                }
                
                break;
            case Direction.Down:
                for (int y = position.y - 1; y >= 0; y--)
                {
                    var temp = GetObjectsAtGridPosition(new Vector2Int(position.x, y), secondaryArray);
                    for (int i = 0; i < temp.Count; i++)
                    {
                        objects.Add(temp[i]);
                    }
                }

                break;
            
            case Direction.Left:
                for (int x = position.x - 1; x >= 0; x--)
                {
                    var temp = GetObjectsAtGridPosition(new Vector2Int(x, position.y), secondaryArray);
                    for (int i = 0; i < temp.Count; i++)
                    {
                        objects.Add(temp[i]);
                    }
                }

                break;
            case Direction.Right:
                for (int x = position.x + 1; x < Width; x++)
                {
                    var temp = GetObjectsAtGridPosition(new Vector2Int(x, position.y), secondaryArray);
                    for (int i = 0; i < temp.Count; i++)
                    {
                        objects.Add(temp[i]);
                    }
                }
                
                break;
            case Direction.UpLeft:
                for (int x = position.x - 1; x >= 0; x--)
                {
                    for (int y = position.y + 1; y < Height; y++)
                    {
                        var temp = GetObjectsAtGridPosition(new Vector2Int(x, y), secondaryArray);
                        for (int i = 0; i < temp.Count; i++)
                        {
                            objects.Add(temp[i]);
                        }
                    }
                }
                
                break;
            case Direction.UpRight:
                for (int x = position.x + 1; x < Width; x++)
                {
                    for (int y = position.y + 1; y < Height; y++)
                    {
                        var temp = GetObjectsAtGridPosition(new Vector2Int(x, y), secondaryArray);
                        for (int i = 0; i < temp.Count; i++)
                        {
                            objects.Add(temp[i]);
                        }
                    }
                }

                break;
            case Direction.DownLeft:
                for (int x = position.x - 1; x >= 0; x--)
                {
                    for (int y = position.y - 1; y >= 0; y--)
                    {
                        var temp = GetObjectsAtGridPosition(new Vector2Int(x, y), secondaryArray);
                        for (int i = 0; i < temp.Count; i++)
                        {
                            objects.Add(temp[i]);
                        }
                    }
                }
                
                break;
            
            case Direction.DownRight:
                for (int x = position.x + 1; x < Width; x++)
                {
                    for (int y = position.y - 1; y >= 0; y--)
                    {
                        var temp = GetObjectsAtGridPosition(new Vector2Int(x, y), secondaryArray);
                        for (int i = 0; i < temp.Count; i++)
                        {
                            objects.Add(temp[i]);
                        }
                    }
                }
                
                break;
        }
        return objects;
    }
    
    public List<GameObject> GetAllObjects(bool secondaryArray = false)
    {
        if (secondaryArray)
        {
            return allObjects2;
        }
        return allObjects;
    }

    public void RemoveObject(GameObject obj, Vector2Int worldPosition, bool secondaryArray = false)
    {
        if (secondaryArray)
        {
            gridArray2[worldPosition.x - BottomLeft.x, worldPosition.y - BottomLeft.y].Remove(obj);
            allObjects2.Remove(obj);
            return;
        }
        gridArray[worldPosition.x - BottomLeft.x, worldPosition.y - BottomLeft.y].Remove(obj);
        allObjects.Remove(obj);
    }
    
    public void AddObject(GameObject obj, Vector2Int worldPosition, bool secondaryArray = false)
    {
        if (worldPosition.x < BottomLeft.x || worldPosition.x >= Width + BottomLeft.x || worldPosition.y < BottomLeft.y || worldPosition.y >= Height + BottomLeft.y)
        {
            Destroy(obj);
            Debug.Log("Object out of the grid. Destroying it.");
            return;
        }

        if (secondaryArray)
        {
            gridArray2[worldPosition.x - BottomLeft.x, worldPosition.y - BottomLeft.y] ??= new List<GameObject>();
            gridArray2[worldPosition.x - BottomLeft.x, worldPosition.y - BottomLeft.y].Add(obj);
            allObjects2.Add(obj);
            return;
        }
        
        gridArray[worldPosition.x - BottomLeft.x, worldPosition.y - BottomLeft.y] ??= new List<GameObject>();
        gridArray[worldPosition.x - BottomLeft.x, worldPosition.y - BottomLeft.y].Add(obj);
        allObjects.Add(obj);
    }
}
