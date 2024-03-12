using System.Collections;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Fader fader;
    [SerializeField] private RectTransform mainMenu;
    [SerializeField] private RectTransform levelSelectMenu;
    [SerializeField] private RectTransform otherMenu;
    [SerializeField] private RectTransform bonusLevelsMenu;
    [SerializeField] private AnimationCurve menuTransitionCurve;
    [SerializeField] private float menuTransitionTime = 1f;
    [SerializeField] private GameObject quitButton;
    
    private Vector3 initialMainMenuPosition;
    private Vector3 initialLevelSelectMenuPosition;
    private Vector3 initialOtherMenuPosition;
    private Vector3 initialBonusLevelsMenuPosition;

    private bool isMoving;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            quitButton.SetActive(false);
        }
        fader.FadeIn();
        initialMainMenuPosition = mainMenu.anchoredPosition;
        initialLevelSelectMenuPosition = levelSelectMenu.anchoredPosition;
        initialOtherMenuPosition = otherMenu.anchoredPosition;
        initialBonusLevelsMenuPosition = bonusLevelsMenu.anchoredPosition;
    }
    
    public void Play()
    {
        fader.TransitionToScene(1, 2);
        MusicPlayer.Instance.FadeOut(3f);
    }

    public void StartLevel(int levelIndex)
    {
        fader.TransitionToScene(levelIndex, 2);
    }
    
    public void ShowLevelSelectMenu()
    {
        StartCoroutine(TransitionMenu(initialMainMenuPosition - initialLevelSelectMenuPosition - (Vector3)mainMenu.anchoredPosition));
    }
    
    public void ShowMainMenu()
    {
        StartCoroutine(TransitionMenu(initialMainMenuPosition - (Vector3)mainMenu.anchoredPosition));
    }
    
    public void ShowOtherMenu()
    {
        StartCoroutine(TransitionMenu(initialMainMenuPosition - initialOtherMenuPosition));
    }
    
    public void ShowBonusLevelsMenu()
    {
        StartCoroutine(TransitionMenu(initialLevelSelectMenuPosition - initialBonusLevelsMenuPosition));
    }
    
    private IEnumerator TransitionMenu(Vector3 movement)
    {
        if (isMoving)
            yield break;
        isMoving = true;
        float time = 0;
        Vector3 mainMenuPosition = mainMenu.anchoredPosition;
        Vector3 levelSelectMenuPosition = levelSelectMenu.anchoredPosition;
        Vector3 otherMenuPosition = otherMenu.anchoredPosition;
        Vector3 bonusLevelsMenuPosition = bonusLevelsMenu.anchoredPosition;
        while (time < menuTransitionTime)
        {
            time += Time.deltaTime;
            mainMenu.anchoredPosition = Vector3.Lerp(Vector3.zero, movement, menuTransitionCurve.Evaluate(time)) + mainMenuPosition;
            levelSelectMenu.anchoredPosition = Vector3.Lerp(Vector3.zero, movement, menuTransitionCurve.Evaluate(time)) + levelSelectMenuPosition;
            otherMenu.anchoredPosition = Vector3.Lerp(Vector3.zero, movement, menuTransitionCurve.Evaluate(time)) + otherMenuPosition;
            bonusLevelsMenu.anchoredPosition = Vector3.Lerp(Vector3.zero, movement, menuTransitionCurve.Evaluate(time)) + bonusLevelsMenuPosition;
            
            yield return null;
        }
        mainMenu.anchoredPosition = movement + mainMenuPosition;
        levelSelectMenu.anchoredPosition = movement + levelSelectMenuPosition;
        otherMenu.anchoredPosition = movement + otherMenuPosition;
        bonusLevelsMenu.anchoredPosition = movement + bonusLevelsMenuPosition;
        isMoving = false;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
