using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusicScript : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;
    private AudioSource musicSource;

    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = musicClip;
        PlayMusic();
    }

    public float GetMusicLength()
    {
        return musicClip.length;
    }

    // No fading in and out required to preserve the raw audio, if required, should manually include the fades
    public void PlayMusic() 
    {
        if (!musicSource.isPlaying)
        { musicSource.Play(); }
    }

    public void PauseMusic()
    {
        if (musicSource.isPlaying)
        { musicSource.Pause(); }
    }

    public void ResumeMusic()
    {
        if (!musicSource.isPlaying)
        { musicSource.UnPause(); }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
