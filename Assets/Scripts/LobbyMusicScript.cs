using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyMusicScript : MonoBehaviour
{
    public static LobbyMusicScript instance;

    [SerializeField] private AudioClip lobbyMusicClip;
    [SerializeField] private float fadeDuration; // Equivalent to transition duration
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
        StartCoroutine(PlayMusic(lobbyMusicClip));
    }

    // Update is called once per frame
    void Update()
    {
        // For manual looping to include fade in and out everytime
        if (!musicSource.isPlaying) 
        {
            StartCoroutine(PlayMusic(lobbyMusicClip));
        }
    }

    private IEnumerator PlayMusic(AudioClip musicClip)
    {
        musicSource.clip = musicClip;
        musicSource.Play();
        MusicFadeIn();

        float currentTime = 0;

        while (currentTime < musicClip.length - fadeDuration)
        {
            currentTime += Time.deltaTime;
            yield return null;
        }

        MusicFadeOut();
    }

    private IEnumerator FadeToVolume(float targetVolume, float fadeDuration, bool destroyObject)
    {
        float currentTime = 0;
        float startVolume = musicSource.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / fadeDuration);
            yield return null;
        }

        if (destroyObject) { Destroy(gameObject); }
    }

    private void MusicFadeIn()
    {
        StartCoroutine(FadeToVolume(1f, fadeDuration, false));
    }

    private void MusicFadeOut()
    {
        StartCoroutine(FadeToVolume(0f, fadeDuration, false));
    }

    // Used when entering a level from level selector
    public void MusicFadeOutAndDestroy()
    {
        StartCoroutine(FadeToVolume(0f, fadeDuration, true));
    }
}
