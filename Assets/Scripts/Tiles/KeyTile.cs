using System;
using UnityEngine;

public class KeyTile : PushableTile
{
    [SerializeField] private int keyIndex;
    [SerializeField] private Color activatedColor;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AudioSource audioSource;
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
        audioSource.Play();
    }
    
    public void Deactivate()
    {
        spriteRenderer.color = startColor;
    }
}
