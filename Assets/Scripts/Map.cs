using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform playerIndicatorTransform;
    private PlayerController playerController;
    private PlayerMovement player;
    private GridManager gridManager;
    private int mapActive = -1;

    private void Awake()
    {
        playerController = new PlayerController();
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        playerController.Player.Map.performed += (ctx) => ToggleMap();
    }

    private void OnEnable()
    {
        playerController.Player.Enable();
    }

    private void OnDisable()
    {
        playerController.Player.Disable();
    }
    
    private void ToggleMap()
    {
        if (player.IsMoving || GameManager.Instance.IsGameEnded)
        {
            return;
        }
        mapActive += 1;
        if (mapActive < gridManager.GridCount)
        {
            playerIndicatorTransform.gameObject.SetActive(true);
            playerIndicatorTransform.position = player.transform.position;
            gridManager.ShowGrid(mapActive);
            player.gameObject.SetActive(false);
        }
        else
        {
            mapActive = -1;
            playerIndicatorTransform.gameObject.SetActive(false);
            gridManager.ResumeGrid();
            player.gameObject.SetActive(true);
        }
    }
}
