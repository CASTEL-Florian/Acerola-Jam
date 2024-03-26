using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionManager : MonoBehaviour
{
    [SerializeField] private Fader fader;
    [SerializeField] private List<TextMeshProUGUI> texts;
    [SerializeField] private float textFadeInTime = 1f;
    [SerializeField] private float textHoldTime = 1f;
    [SerializeField] private float textFadeOutTime = 2f;
    [SerializeField] private float timeBetweenTexts = 0.5f;
    [SerializeField] private float transitionTime = 2f;
    [SerializeField] private bool fadeInAudioOnEnd;
    private void Start()
    {
        fader.FadeIn();
        StartCoroutine(TransitionCoroutine());
    }
    
    public void TransitionToMainMenu()
    {
        fader.TransitionToScene(0);
    }

    private IEnumerator TransitionCoroutine()
    {
        for (int i = 0; i < texts.Count; i++)
        {
            yield return FadeInText(texts[i]);
            yield return new WaitForSeconds(textHoldTime);
            if (i < texts.Count - 1)
            {
                yield return FadeOutText(texts[i]);
                yield return new WaitForSeconds(timeBetweenTexts);
            }
        }
        yield return new WaitForSeconds(transitionTime);
        
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        fader.TransitionToScene(nextSceneIndex, 2);

        if (fadeInAudioOnEnd)
        {
            MusicPlayer.Instance.PrepareFadeInAtNextScene();
        }
    }
    
    private IEnumerator FadeInText(TextMeshProUGUI text)
    {
        text.gameObject.SetActive(true);
        float elapsedTime = 0;
        while (elapsedTime < textFadeInTime)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(0, 1, elapsedTime / textFadeInTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
    }
    
    private IEnumerator FadeOutText(TextMeshProUGUI text)
    {
        float elapsedTime = 0;
        while (elapsedTime < textFadeOutTime)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(1, 0, elapsedTime / textFadeOutTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
    }
    
    public void SkipTransition()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        fader.TransitionToScene(nextSceneIndex, 2);

        if (fadeInAudioOnEnd)
        {
            MusicPlayer.Instance.PrepareFadeInAtNextScene();
        }
    }
}
