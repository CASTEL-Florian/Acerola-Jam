using System.Collections;
using UnityEngine;

public class EndMenu : MonoBehaviour
{
    [SerializeField] private Fader fader;
    [SerializeField] private GameObject quitButton;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            quitButton.SetActive(false);
        }
        fader.FadeIn();
        MusicPlayer.Instance.SceneLoaded();
    }
    
    public void TransitionToMainMenu()
    {
        fader.TransitionToScene(0);
    }
    
    public void TransitionToExtraLevels()
    {
        MusicPlayer.Instance.ExtraLevels();
        fader.TransitionToScene(0);
    }
    
    public void Quit()
    {
        Application.Quit();
    }
}
