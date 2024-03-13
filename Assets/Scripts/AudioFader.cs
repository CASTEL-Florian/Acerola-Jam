using System.Collections;
using UnityEngine.Audio;
using UnityEngine;

public static class AudioFader
{
    public static IEnumerator StartFade(AudioMixer audioMixer, string exposedParam, float duration, float targetVolume)
    {
        // start after 1 frame in case the fade starts at the first frame after loading the scene and there is a lag spike.
        yield return null; 
        
        float currentTime = 0;
        audioMixer.GetFloat(exposedParam, out float currentVol);
        currentVol = Mathf.Pow(10, currentVol / 20);
        float targetValue = Mathf.Clamp(targetVolume, 0.0001f, 1);
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float newVol = Mathf.Lerp(currentVol, targetValue, currentTime / duration);
            audioMixer.SetFloat(exposedParam, Mathf.Log10(newVol) * 20);
            yield return null;
        }
        audioMixer.SetFloat(exposedParam, Mathf.Log10(targetValue) * 20);
    }
}
