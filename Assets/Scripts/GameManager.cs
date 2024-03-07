using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Fader fader;
    [SerializeField] private float faderFadeOutTime = 1f;
    [SerializeField] private Undo undo;
    
    private bool loadingNextScene = false;
    

    private PlayerController playerController;

    private PlayerMovement player;
    public event Action OnGameEnd;

    public bool IsGameEnded { get; private set; }
    public static GameManager Instance { get; private set; }
    void Awake()
    {
        if (Instance == null) { Instance = this; } else if (Instance != this) { Destroy(this); }
    }

    private void Start()
    {
        player = FindObjectOfType<PlayerMovement>();
        fader.FadeIn();
        playerController = new PlayerController();
        playerController.Player.Enable();
        playerController.Player.Restart.performed += _ => RestartScene();
        playerController.Player.Undo.performed += _ => Undo();
    }

    private void OnDestroy()
    {
        playerController.Player.Disable();
    }

    public void LoadNextScene()
    {
        if (loadingNextScene)
            return;
        OnGameEnd?.Invoke();
        IsGameEnded = true;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }

        fader.TransitionToScene(nextSceneIndex, faderFadeOutTime);
        loadingNextScene = true;
    }

    public void RestartScene()
    {
        if (loadingNextScene)
            return;
        OnGameEnd?.Invoke();
        IsGameEnded = true;
        fader.TransitionToScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void Record()
    {
        undo.Record();
    }
    
    public void Undo()
    {
        if (!player.CanMove || IsGameEnded)
            return;
        undo.UndoLast();
    }
}
