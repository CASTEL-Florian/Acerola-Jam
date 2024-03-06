using System;
using UnityEngine;

public class KeyTile : PushableTile
{
    [SerializeField] private int keyIndex;
    [SerializeField] private Color activatedColor;
    
    public int KeyIndex => keyIndex;
    private Color startColor;

    private void Start()
    {
        startColor = GetComponent<SpriteRenderer>().color;
    }

    public void Activate()
    {
        GetComponent<SpriteRenderer>().color = activatedColor;
    }
    
    public void Deactivate()
    {
        GetComponent<SpriteRenderer>().color = startColor;
    }
}
