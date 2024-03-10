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
    [SerializeField] private GameObject pauseMenu;
    
    private bool loadingNextScene = false;
    public bool IsMapOpen { get; set; }

    private PlayerController playerController;

    private PlayerMovement player;
    public event Action OnGameEnd;

    public bool IsGameEnded { get; private set; }
    
    public bool IsGamePaused => pauseMenu.activeSelf;
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
        playerController.Player.Pause.performed += _ => TogglePauseMenu();
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
    
    private void Undo()
    {
        if (!player.CanMove || IsGameEnded || IsMapOpen || pauseMenu.activeSelf)
            return;
        undo.UndoLast();
    }

    public void TogglePauseMenu()
    {
        if (IsGameEnded)
            return;
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }
    
    public void TransitionToMainMenu()
    {
        fader.TransitionToScene(0);
        loadingNextScene = true;
    }
}
