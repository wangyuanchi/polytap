using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class LevelMusicScript : MonoBehaviour
{
    [Header("Beat Map")]
    public float beatMapEndTime; // Make sure the end timing set is at least 0.5s after the last correct input to prevent clashes
                                 // where wrong user input on last note is performed after level complete

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip[] musicClip;
    //levelArray is used to find the position of the corresponding music clip for that level
    private string[] levelArray = {"L1","L2"};

    [Header("Managers")]
    [SerializeField] private GameObject PracticeManager;

    // Start is called before the first frame update
    void Start()
    {
        // Gets the current level, then changes the clip to the one for that level
        string level = PlayerPrefs.GetString("Level");
        musicSource.clip = musicClip[Array.FindIndex(levelArray,level => level.Contains(level))];
        // Check for incorrect input
        if (beatMapEndTime < 0 || beatMapEndTime > musicSource.clip.length)
        { Debug.Log("Invalid Beat Map End Time!"); }

        // Non-practice mode playing, practice mode playing is done in PracticeManagerScript
        if (!PracticeManagerScript.practiceMode)    
        {
            PlayMusic();
        }
    }

    // [PRACTICE MODE] MAIN METHOD
    // Set the new time position and play
    public void SkipToTime(float timeSkipped)
    {
        StopMusic();
        musicSource.time = Mathf.Clamp(timeSkipped, 0f, musicSource.clip.length);
        PlayMusic();
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

    public float getCurrentTimeStamp()
    {
        if (musicSource == null) return 0; // Music has not yet been set and start playing
        return musicSource.time;
    }
}
