using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Fader fader;
    [SerializeField] private int nextSceneIndex;
    
    private bool loadingNextScene = false;

    private PlayerController playerController;
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null) { Instance = this; } else if (Instance != this) { Destroy(this); }
    }

    private void Start()
    {
        fader.FadeIn();
        playerController = new PlayerController();
        playerController.Player.Enable();
        playerController.Player.Restart.performed += _ => RestartScene();
    }

    private void OnDestroy()
    {
        playerController.Player.Disable();
    }

    public void LoadNextScene()
    {
        if (loadingNextScene)
            return;
        fader.TransitionToScene(nextSceneIndex);
        loadingNextScene = true;
    }

    public void RestartScene()
    {
        if (loadingNextScene)
            return;
        fader.TransitionToScene(SceneManager.GetActiveScene().buildIndex);
    }
}
