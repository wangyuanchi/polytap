using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioSlidersScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;

    // Start is called before the first frame update
    void Start()
    {
        LoadSliders();
    }

    private void LoadSliders()
    {
        musicSlider.value = PlayerPrefs.GetFloat("Music Volume");
        SFXSlider.value = PlayerPrefs.GetFloat("SFX Volume");
    }

    public void SetMusicVolume()
    {
        float musicVolume = musicSlider.value;
        audioMixer.SetFloat("Music Volume", Mathf.Log10(musicVolume) * 25);
        PlayerPrefs.SetFloat("Music Volume", musicVolume);
    }

    public void SetSFXVolume()
    {
        float SFXVolume = SFXSlider.value;
        audioMixer.SetFloat("SFX Volume", Mathf.Log10(SFXVolume) * 25);
        PlayerPrefs.SetFloat("SFX Volume", SFXVolume);
    }
}
