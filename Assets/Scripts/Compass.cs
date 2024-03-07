using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    
    public class ImagePair
    {
        public Image image1;
        public Image image2;
    }
    [SerializeField] private List<Color> gridColors;
    [SerializeField] private List<Transform> thingsToRotate;

    [SerializeField] private Image up1;
    [SerializeField] private Image up2;
    
    [SerializeField] private Image right1;
    [SerializeField] private Image right2;
    
    [SerializeField] private Image down1;
    [SerializeField] private Image down2;
    
    [SerializeField] private Image left1;
    [SerializeField] private Image left2;
    
    [SerializeField] private Image upLeft;
    [SerializeField] private Image upRight;
    [SerializeField] private Image downLeft;
    [SerializeField] private Image downRight;
    
    private PlayerMovement player;
    
    private Dictionary<Direction, ImagePair> directionToImages = new Dictionary<Direction, ImagePair>();

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        directionToImages.Add(Direction.Up, new ImagePair {image1 = up1, image2 = up2});
        directionToImages.Add(Direction.Right, new ImagePair {image1 = right1, image2 = right2});
        directionToImages.Add(Direction.Down, new ImagePair {image1 = down1, image2 = down2});
        directionToImages.Add(Direction.Left, new ImagePair {image1 = left1, image2 = left2});
        
        directionToImages.Add(Direction.UpRight, new ImagePair {image1 = upRight, image2 = upRight});
        directionToImages.Add(Direction.UpLeft, new ImagePair {image1 = upLeft, image2 = upLeft});
        directionToImages.Add(Direction.DownRight, new ImagePair {image1 = downRight, image2 = downRight});
        directionToImages.Add(Direction.DownLeft, new ImagePair {image1 = downLeft, image2 = downLeft});
        
    }

    private void Update()
    {
        float angle = player.transform.eulerAngles.z;
        foreach (var thing in thingsToRotate)
        {
            thing.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    public void UpdateColors(Vector2Int gridsBefore, Vector2Int currentGrids, Vector2Int gridsAfter)
    {
        Direction direction = player.CurrentDirection;

        Direction[] directions = new[]
        {
            TurnDirection(direction, PlayerMovement.Turn.Right, 4),
            TurnDirection(direction, PlayerMovement.Turn.Right, 3),
            TurnDirection(direction, PlayerMovement.Turn.Right, 2),
            TurnDirection(direction, PlayerMovement.Turn.Right, 1),
            direction,
            TurnDirection(direction, PlayerMovement.Turn.Left, 1),
            TurnDirection(direction, PlayerMovement.Turn.Left, 2),
            TurnDirection(direction, PlayerMovement.Turn.Left, 3),
        };
        directionToImages[directions[0]].image2.color = gridColors[gridsBefore.x];
        directionToImages[directions[1]].image1.color = gridColors[gridsBefore.x];
        directionToImages[directions[2]].image1.color = gridColors[gridsBefore.x];
        directionToImages[directions[2]].image2.color = gridColors[gridsBefore.y];
        directionToImages[directions[3]].image1.color = gridColors[currentGrids.x];
        directionToImages[directions[4]].image1.color = gridColors[currentGrids.x];
        directionToImages[directions[4]].image2.color = gridColors[currentGrids.y];
        directionToImages[directions[5]].image1.color = gridColors[currentGrids.y];
        directionToImages[directions[6]].image1.color = gridColors[gridsAfter.x];
        directionToImages[directions[6]].image2.color = gridColors[gridsAfter.y];
        directionToImages[directions[7]].image1.color = gridColors[gridsAfter.y];
        directionToImages[directions[0]].image1.color = gridColors[gridsAfter.y];

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
}
