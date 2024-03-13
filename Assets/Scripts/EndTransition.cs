using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTransition : MonoBehaviour
{
    [SerializeField] private List<string> texts;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Fader fader;
    [SerializeField] private float timaBetweenCharacters = 0.1f;
    [SerializeField] private float timeBetweenTexts = 2;
    
    private void Start()
    {
        StartCoroutine(TransitionCoroutine());
        fader.FadeIn();
    }

    private IEnumerator TransitionCoroutine()
    {
        yield return null;
        MusicPlayer.Instance.FadeOut(2f);
        yield return new WaitForSeconds(4);
        for (int i = 0; i < texts.Count; i++)
        {
            text.text = "";
            for (int j = 0; j < texts[i].Length; j++)
            {
                text.text += texts[i][j];
                yield return new WaitForSeconds(timaBetweenCharacters);
            }
            yield return new WaitForSeconds(timeBetweenTexts);
        }
        
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex >= SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }
        fader.TransitionToScene(nextSceneIndex, 2);
        MusicPlayer.Instance.PrepareFadeInAtNextScene();
    }
}
