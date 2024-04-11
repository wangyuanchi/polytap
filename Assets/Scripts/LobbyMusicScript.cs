using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMusicScript : MonoBehaviour
{
    public AudioClip musicClip;
    public static LobbyMusicScript instance;

    private AudioSource musicSource;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        musicSource = GetComponent<AudioSource>();
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
