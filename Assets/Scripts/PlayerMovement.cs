using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public enum Turn
    {
        Left,
        Right
    }
    
    
    public float speed = 2.0f;
    private Vector2Int targetPosition;
    private bool moving;
    private PlayerController playerController;
    private GridManager gridManager;
    public event Action<Turn> OnTurn;
    public event Action<Vector2Int> OnFroward;

    public Direction CurrentDirection { get; private set; } = Direction.Right;
    
    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        playerController = new PlayerController();
        playerController.Player.Enable();
        playerController.Player.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
        playerController.Player.Rotate.performed += ctx => Rotate(ctx.ReadValue<float>());
    }
    


    private void Move(Vector2 movement)
    {
        if (moving)
        {
            return;
        }

        if (MovementBackward(movement))
        {
            return;
        }
        
        Vector2Int currentPosition =
            new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        targetPosition = new Vector2Int(Mathf.RoundToInt(currentPosition.x + movement.x), Mathf.RoundToInt(currentPosition.y + movement.y));
        
        List<GameObject> objects = gridManager.GetObjectsAtPosition(targetPosition);
        foreach (GameObject obj in objects)
        {
            Tile tile = obj.GetComponent<Tile>();
            if (tile != null)
            {
                if (!tile.TryWalkingOnTile(CurrentDirection))
                {
                    return;
                }
            }
        }
        
        moving = true;
        if (movement.x > 0 && CurrentDirection == Direction.Right)
        {
            OnFroward?.Invoke(currentPosition);
        }
        else if (movement.x < 0 && CurrentDirection == Direction.Left)
        {
            OnFroward?.Invoke(currentPosition);
        }
        else if (movement.y > 0 && CurrentDirection == Direction.Up)
        {
            OnFroward?.Invoke(currentPosition);
        }
        else if (movement.y < 0 && CurrentDirection == Direction.Down)
        {
            OnFroward?.Invoke(currentPosition);
        }
        StartCoroutine(SmoothMovementCoroutine());
    }

    private bool MovementBackward(Vector2 movement)
    {
        if (movement.x < 0 && CurrentDirection == Direction.Right)
        {
            return true;
        }
        if (movement.x > 0 && CurrentDirection == Direction.Left)
        {
            return true;
        }
        if (movement.y < 0 && CurrentDirection == Direction.Up)
        {
            return true;;
        }
        if (movement.y > 0 && CurrentDirection == Direction.Down)
        {
            return true;
        }
        return false;
    }
    
    
    private void Rotate(float rotation)
    {
        if (moving)
        {
            return;
        }
        if (rotation > 0)
        {
            transform.Rotate(0, 0, -90);
            switch (CurrentDirection)
            {
                case Direction.Up:
                    CurrentDirection = Direction.Right;
                    break;
                case Direction.Down:
                    CurrentDirection = Direction.Left;
                    break;
                case Direction.Left:
                    CurrentDirection = Direction.Up;
                    break;
                case Direction.Right:
                    CurrentDirection = Direction.Down;
                    break;
            }
        }
        else
        {
            transform.Rotate(0, 0, 90);
            switch (CurrentDirection)
            {
                case Direction.Up:
                    CurrentDirection = Direction.Left;
                    break;
                case Direction.Down:
                    CurrentDirection = Direction.Right;
                    break;
                case Direction.Left:
                    CurrentDirection = Direction.Down;
                    break;
                case Direction.Right:
                    CurrentDirection = Direction.Up;
                    break;
            }
        }
        OnTurn?.Invoke(rotation > 0 ? Turn.Right : Turn.Left);
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
        moving = false;
    }
}
