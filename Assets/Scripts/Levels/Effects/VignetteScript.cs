using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class VignetteScript : MonoBehaviour
{
    [Header("Intensity")]
    [SerializeField] private float peakIntensity2;
    [SerializeField] private float peakIntensity1;
    [SerializeField] private float peakIntensity0;
    [SerializeField] private float targetIntensity2;
    [SerializeField] private float targetIntensity1;
    [SerializeField] private float targetIntensity0;

    private bool enableVignette;
    private float intensity;
    private PostProcessVolume volume;
    private Vignette vignette;
    private Coroutine ChangeVignetteIntensityCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        enableVignette = PlayerPrefs.GetString("Vignette") == "true" ? true : false; 
       
        volume = GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings<Vignette>(out vignette);

        if (!enableVignette)
        {
            vignette.enabled.Override(false);
            return;
        }

        if (!vignette)
        {
            Debug.Log("Error: Empty Vignette");
        }
        else if (PlayerPrefs.GetString("Mode") == "N")
        {
            vignette.enabled.Override(false);
        }
        else
        {
            vignette.enabled.Override(true);
            vignette.intensity.Override(targetIntensity1);
        }
    }

    private IEnumerator ChangeVignetteIntensity(float peakIntensity, float targetIntensity)
    {
        vignette.enabled.Override(true);
        float initialIntensity = vignette.intensity;
        float fadeInDuration = 0.2f;
        float pauseDuration = 0.5f;
        float fadeOutDuration = 3f;
        float time = 0f;

        // Fade into peak intensity
        while (time < fadeInDuration)
        {
            time += Time.deltaTime;
            intensity = Mathf.Lerp(initialIntensity, peakIntensity, time / fadeInDuration);
            vignette.intensity.Override(intensity);
            yield return null;
        }

        yield return new WaitForSeconds(pauseDuration);

        // Reset variables for fading out
        time = 0f;
        initialIntensity = vignette.intensity;

        // Fade out to target intensity
        while (time < fadeOutDuration)
        {
            time += Time.deltaTime;
            intensity = Mathf.Lerp(initialIntensity, targetIntensity, time / fadeOutDuration);
            vignette.intensity.Override(intensity);
            yield return null;
        }
    }

    public void SetVignette(int currentHealth)
    {
        if (ChangeVignetteIntensityCoroutine != null)
        {
            StopCoroutine(ChangeVignetteIntensityCoroutine);
        }

        if (currentHealth == 2)
        {
            ChangeVignetteIntensityCoroutine = StartCoroutine(ChangeVignetteIntensity(peakIntensity2, targetIntensity2));
        }
        else if (currentHealth == 1) 
        {
            ChangeVignetteIntensityCoroutine = StartCoroutine(ChangeVignetteIntensity(peakIntensity1, targetIntensity1));
        }
        else if (currentHealth == 0)
        {
            ChangeVignetteIntensityCoroutine = StartCoroutine(ChangeVignetteIntensity(peakIntensity0, targetIntensity0));
        }
    }
}
