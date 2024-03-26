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

    public enum MoveAction
    {
        None,
        RotateLeft,
        RotateRight,
        MoveLeft,
        MoveRight,
        MoveUp,
        MoveDown
    }


    public float speed = 2.0f;
    public float rotationDuration = 0.2f;
    [SerializeField] private Transform rightMask;
    [SerializeField] private Transform leftMask;
    [SerializeField] private float pauseDuration = 0.1f;
    private Vector2Int targetPosition;
    private bool moving;
    private PlayerController playerController;
    private GridManager gridManager;
    public event Action<Turn> OnTurn;
    public event Action<Vector2Int> OnFroward;
    
    public bool IsMoving => moving;
    
    public bool  CanMove => !moving && !paused;
    
    private bool paused = false;

    public Direction CurrentDirection { get; private set; } = Direction.Right;

    private MoveAction moveActionBuffer = MoveAction.None; 
    
    private MoveAction latestMoveAction = MoveAction.None;

    private void Awake()
    {
        playerController = new PlayerController();
    }

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        
        playerController.Player.MoveHorizontal.performed += MoveHorizontal;
        playerController.Player.MoveVertical.performed += MoveVertical;
        playerController.Player.Rotate.performed += ctx => Rotate(ctx.ReadValue<float>());
        GameManager.Instance.OnGameEnd += () => playerController.Player.Disable();
    }

    private void OnEnable()
    {
        playerController.Player.Enable();
    }

    private void OnDisable()
    {
        playerController.Player.Disable();
    }


    private void MoveHorizontal(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        Move(new Vector2(value, 0));
        latestMoveAction = value > 0 ? MoveAction.MoveRight : MoveAction.MoveLeft;
    }
    private void MoveVertical(InputAction.CallbackContext ctx)
    {
        float value = ctx.ReadValue<float>();
        Move(new Vector2(0, value));   
        latestMoveAction = value > 0 ? MoveAction.MoveUp : MoveAction.MoveDown;
    }
    private void Move(Vector2 movement)
    {
        if (GameManager.Instance.IsGamePaused)
        {
            return;
        }
        if (moving || paused)
        {
            if (moveActionBuffer == MoveAction.None)
            {
                if (movement.x > 0)
                {
                    moveActionBuffer = MoveAction.MoveRight;
                }
                else if (movement.x < 0)
                {
                    moveActionBuffer = MoveAction.MoveLeft;
                }
                else if (movement.y > 0)
                {
                    moveActionBuffer = MoveAction.MoveUp;
                }
                else if (movement.y < 0)
                {
                    moveActionBuffer = MoveAction.MoveDown;
                }
            }
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
                if (!tile.TryWalkingOnTile(MovementDirection(movement), true))
                {
                    return;
                }
            }
        }
        GameManager.Instance.Record();
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

    private Direction MovementDirection(Vector2 movement)
    {
        if (movement.x > 0)
        {
            return Direction.Right;
        }
        if (movement.x < 0)
        {
            return Direction.Left;
        }
        if (movement.y > 0)
        {
            return Direction.Up;
        }
        if (movement.y < 0)
        {
            return Direction.Down;
        }
        return Direction.Right;
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
        latestMoveAction = rotation > 0 ? MoveAction.RotateRight : MoveAction.RotateLeft;
        if (GameManager.Instance.IsGamePaused)
        {
            return;
        }
        if (moving || paused)
        {
            if (moveActionBuffer == MoveAction.None)
            {
                if (rotation > 0)
                {
                    moveActionBuffer = MoveAction.RotateRight;
                }
                else if (rotation < 0)
                {
                    moveActionBuffer = MoveAction.RotateLeft;
                }
            }
            return;
        }
        
        GameManager.Instance.Record();

        moving = true;
        if (rotation > 0)
        {
            StartCoroutine(SmoothRotationCoroutine(-90));
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
            StartCoroutine(SmoothRotationCoroutine(90));
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

    private IEnumerator SmoothMovementCoroutine()
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
        paused = true;
        moving = false;
        yield return new WaitForSeconds(pauseDuration);
        paused = false;
    }


    private IEnumerator SmoothRotationCoroutine(float rotation)
    {
        
        float t = 0;
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + rotation);
        while (t < 1)
        {
            t += Time.deltaTime / rotationDuration;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return null;
        }
        transform.rotation = endRotation;
        paused = true;
        moving = false;
        yield return new WaitForSeconds(pauseDuration);
        paused = false;
    }

    public void UpdateDirection()
    {
        Vector3 forward = transform.right;
        if (forward.x > 0.5f)
        {
            CurrentDirection = Direction.Right;
        }
        else if (forward.x < -0.5f)
        {
            CurrentDirection = Direction.Left;
        }
        else if (forward.y > 0.5f)
        {
            CurrentDirection = Direction.Up;
        }
        else if (forward.y < -0.5f)
        {
            CurrentDirection = Direction.Down;
        }
    }

    private void Update()
    {
        if (GameManager.Instance.IsGameEnded || !CanMove)
        {
            return;
        }
        if (moveActionBuffer != MoveAction.None)
        {
            switch (moveActionBuffer)
            {
                case MoveAction.RotateLeft:
                    Rotate(-1);
                    break;
                case MoveAction.RotateRight:
                    Rotate(1);
                    break;
                case MoveAction.MoveUp:
                    Move(new Vector2(0, 1));
                    break;
                case MoveAction.MoveDown:
                    Move(new Vector2(0, -1));
                    break;
                case MoveAction.MoveLeft:
                    Move(new Vector2(-1, 0));
                    break;
                case MoveAction.MoveRight:
                    Move(new Vector2(1, 0));
                    break;
            }
            moveActionBuffer = MoveAction.None;
        }
        else if (playerController.Player.MoveHorizontal.ReadValue<float>() != 0 || playerController.Player.MoveVertical.ReadValue<float>() != 0 || playerController.Player.Rotate.ReadValue<float>() != 0)
        {
            if (latestMoveAction == MoveAction.MoveLeft || latestMoveAction == MoveAction.MoveRight)
            {
                Move(new Vector2(playerController.Player.MoveHorizontal.ReadValue<float>(), 0));
            }
            else if (latestMoveAction == MoveAction.MoveUp || latestMoveAction == MoveAction.MoveDown)
            {
                Move(new Vector2(0, playerController.Player.MoveVertical.ReadValue<float>()));
            }
            else if (latestMoveAction == MoveAction.RotateLeft || latestMoveAction == MoveAction.RotateRight)
            {
                Rotate(playerController.Player.Rotate.ReadValue<float>());
            }
        }
    }
}
