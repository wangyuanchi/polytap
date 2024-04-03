using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = audioClip;
        PlayAudio();
    }

    public void PlayAudio()
    {
        if (!audioSource.isPlaying)
        { audioSource.Play(); }
    }

    public void PauseAudio()
    {
        if (audioSource.isPlaying)
        { audioSource.Pause(); }
    }

    public void ResumeAudio()
    {
        if (!audioSource.isPlaying)
        { audioSource.UnPause(); }
    }

    // Stopped audio cannot be resumed
    public void StopAudio()
    {
        audioSource.Stop();
    }
}
