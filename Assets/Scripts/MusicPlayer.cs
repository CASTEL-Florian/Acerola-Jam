using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance { get; private set; }
    
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private float fadeInTime = 1f;
    void Awake()
    {
        if (Instance == null) { Instance = this; } else if (Instance != this) { Destroy(gameObject); }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        audioMixer.SetFloat("MusicVolume", -80);
        StartCoroutine(AudioFader.StartFade(audioMixer, "MusicVolume", fadeInTime, 1f));
    }
}
