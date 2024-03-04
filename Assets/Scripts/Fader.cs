using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Fader : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] public float fadeInTime;
    [SerializeField] public float fadeOutTime;
    public bool transitioning { get; private set; }
    public bool fading { get; private set; }
    private Coroutine routine;

    public void FadeOut()
    {
        if (transitioning)
            return;
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(FadeOutRoutine(fadeOutTime));
    }
    public void FadeIn()
    {
        if (transitioning)
            return;
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(FadeInRoutine(fadeInTime));
    }
    private IEnumerator FadeOutRoutine(float time)
    {
        fading = true;
        image.color = Color.clear;
        while (image.color.a < 1)
        {
            Color imageColor = image.color;
            imageColor.a += Time.unscaledDeltaTime / time;
            image.color = imageColor;
            yield return null;
        }
        fading = false;
    }

    private IEnumerator FadeInRoutine(float time)
    {
        fading = true;
        image.color = Color.black;
        while (image.color.a > 0)
        {
            Color imageColor = image.color;
            imageColor.a -= Time.unscaledDeltaTime / time;
            image.color = imageColor;
            yield return null;
        }
        fading = false;
    }

    public void TransitionToScene(int sceneIndex, float fadeOutDuration = 1)
    {
        if (transitioning)
            return;
        transitioning = true;
        if (routine != null)
        {
            StopCoroutine(routine);
        }
        routine = StartCoroutine(TransitionRoutine(sceneIndex, fadeOutDuration));
    }
    private IEnumerator TransitionRoutine(int sceneIndex, float fadeOutDuration)
    {
        yield return FadeOutRoutine(fadeOutDuration);
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneIndex);
    }
}
