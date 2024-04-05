using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public AudioClip musicClip;
    public GameObject musicObject;

    private AudioSource musicSource;

    // Start is called before the first frame update
    void Start()
    {
        musicSource = musicObject.GetComponent<AudioSource>();
        musicSource.clip = musicClip;
        PlayMusic();
    }

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

    // Stopped music cannot be resumed, and must start from the beginning
    public void StopMusic()
    {
        musicSource.Stop();
    }
}
