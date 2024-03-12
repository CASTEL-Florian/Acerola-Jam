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
    public bool IsActivated { get; private set; }
    
    
    

    protected override void Start()
    {
        base.Start();
        startColor = spriteRenderer.color;
    }

    public void Activate(bool silent = false)
    {
        if (IsActivated)
        {
            return;
        }
        IsActivated = true;
        spriteRenderer.color = activatedColor;
        if (!silent)
        {
            audioSource.Play();
        }
    }
    
    public void Deactivate()
    {
        IsActivated = false;
        spriteRenderer.color = startColor;
    }
}
