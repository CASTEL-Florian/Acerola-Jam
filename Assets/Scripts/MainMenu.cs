using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Fader fader;

    private void Start()
    {
        fader.FadeIn();
    }

    public void StartLevel(int levelIndex)
    {
        fader.TransitionToScene(levelIndex, 2);
    }
}
