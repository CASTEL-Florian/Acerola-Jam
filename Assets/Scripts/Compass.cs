using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    [SerializeField] private List<Color> gridColors;
    [SerializeField] private Transform halfCircle;
    
    private PlayerMovement player;

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update()
    {
        float angle = player.transform.eulerAngles.z;
        halfCircle.eulerAngles = new Vector3(0, 0, angle);
    }
}
