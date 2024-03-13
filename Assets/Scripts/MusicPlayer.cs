using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float fadeInTime = 1f;
    
    Coroutine audioFadeCoroutine;

    private bool extraLevels; // see note at the bottom
    void Awake()
    {
        if (Instance == null) { Instance = this; } else if (Instance != this) { Destroy(gameObject); }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        audioMixer.SetFloat("MusicVolume", -80);
        audioFadeCoroutine = StartCoroutine(AudioFader.StartFade(audioMixer, "MusicVolume", fadeInTime, 1f));
    }

    public void FadeOut(float fadeOutTime)
    {
        if (audioFadeCoroutine != null)
        {
            StopCoroutine(audioFadeCoroutine);
        }
        audioFadeCoroutine = StartCoroutine(AudioFader.StartFade(audioMixer, "MusicVolume", fadeOutTime, 0.0001f));
    }
    
    public void FadeIn()
    {
        if (audioFadeCoroutine != null)
        {
            StopCoroutine(audioFadeCoroutine);
        }
        audioFadeCoroutine = StartCoroutine(AudioFader.StartFade(audioMixer, "MusicVolume", fadeInTime, 1f));
    }

    // This shouldn't be there. I should have a separate script for this. But since the Music player is not destroyed between scenes...
    public void ExtraLevels()
    {
        extraLevels = true;
    }

    public bool CheckExtraLevels()
    {
        bool temp = extraLevels;
        extraLevels = false;
        return temp;
    }
}
