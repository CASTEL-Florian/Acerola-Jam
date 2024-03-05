using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableTile : Tile
{
    private GridManager gridManager;
    private Vector2Int targetPosition;
    private float speed = 2.0f;
    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        speed = FindObjectOfType<PlayerMovement>().speed;
    }

    public override bool TryWalkingOnTile(Direction direction)
    {
        Vector2Int currentPosition =
            new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        Vector2 movement = Vector2.zero;
        switch (direction)
        {
            case Direction.Right:
                movement = Vector2.right;
                break;
            case Direction.Up:
                movement = Vector2.up;
                break;
            case Direction.Left:
                movement = Vector2.left;
                break;
            case Direction.Down:
                movement = Vector2.down;
                break;
        }
        targetPosition = new Vector2Int(Mathf.RoundToInt(currentPosition.x + movement.x), Mathf.RoundToInt(currentPosition.y + movement.y));
        
        List<GameObject> objects = gridManager.GetObjectsAtPosition(targetPosition);
        foreach (GameObject obj in objects)
        {
            Tile tile = obj.GetComponent<Tile>();
            if (tile != null)
            {
                if (!tile.TryWalkingOnTile(direction))
                {
                    return false;
                }
            }
        }
        gridManager.MoveObject(gameObject, currentPosition, targetPosition);
        StartCoroutine(SmoothMovementCoroutine());
        return true;
    }
    
    IEnumerator SmoothMovementCoroutine()
    {
        Vector3 target = new Vector3(targetPosition.x, targetPosition.y, 0);
        float remainingDistance = (transform.position - target).sqrMagnitude;

        while (remainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            transform.position = newPosition;
            remainingDistance = (transform.position - target).sqrMagnitude;
            yield return null;
        }
        transform.position = target;
    }
}