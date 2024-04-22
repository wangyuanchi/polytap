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
        musicSource.clip = lobbyMusicClip;

        if (PlayerPrefs.GetString("Lobby Music") == "true")
        {
            PlayLobbyMusic();
        }
    }

    public void PlayLobbyMusic()
    {
        musicSource.Play();
    }

    public void StopLobbyMusic()
    {
        musicSource.Stop();
    }

    private IEnumerator FadeToVolume(float targetVolume, float fadeDuration)
    {
        float currentTime = 0;
        float startVolume = musicSource.volume;

        while (currentTime < fadeDuration)
        {
            currentTime += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, targetVolume, currentTime / fadeDuration);
            yield return null;
        }
    }

    // Destroy object so that it is not carried into the level
    private IEnumerator WaitAndDestroy(float duration)
    { 
        yield return new WaitForSeconds(duration);
        Destroy(gameObject);
    }

    public void MusicFadeOutAndDestroy()
    {
        StartCoroutine(FadeToVolume(0f, fadeDuration));
        StartCoroutine(WaitAndDestroy(fadeDuration));
    }
}
