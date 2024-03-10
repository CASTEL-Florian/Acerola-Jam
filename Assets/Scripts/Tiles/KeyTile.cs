using System;
using UnityEngine;

public class KeyTile : PushableTile
{
    [SerializeField] private int keyIndex;
    [SerializeField] private Color activatedColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    public int KeyIndex => keyIndex;
    private Color startColor;

    protected override void Start()
    {
        base.Start();
        startColor = spriteRenderer.color;
    }

    public void Activate()
    {
        spriteRenderer.color = activatedColor;
    }
    
    public void Deactivate()
    {
        spriteRenderer.color = startColor;
    }
}
