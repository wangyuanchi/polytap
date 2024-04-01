using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip audioClip;
    public void PlayAudio()
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }
}
