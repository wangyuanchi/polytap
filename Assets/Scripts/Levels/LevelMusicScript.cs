using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LevelMusicScript : MonoBehaviour
{
    [Header("Beat Map")]
    public float beatMapEndTime; 

    [Header("Music")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip emptyMusic;

    [Header("Managers")]
    [SerializeField] private GameObject PracticeManager;

    // Start is called before the first frame update
    void Start()
    {
        LoadMusic();
        LoadBeatMapEndTime();

        // Non-practice mode playing, practice mode playing is done in PracticeManagerScript
        if (!PracticeManagerScript.practiceMode)    
        {
            PlayMusic();
        }
    }

    private void LoadMusic()
    {
        // Gets the current level, then references the scriptable object for the music
        string level = StaticInformation.level;

        if (level == null)
        {
            Debug.Log("No level music loaded.");
            musicSource.clip = emptyMusic; // Don't play sounds if starting directly from Level.unity scene,
                                           // but still fill the audio source with a clip to prevent nullexceptionerror
        }
        else
        {
            LevelDataScriptableObject scriptableObjectInstance = Resources.Load<LevelDataScriptableObject>($"LevelData\\{level}");
            musicSource.clip = scriptableObjectInstance.levelMusic;
        }
    }

    private void LoadBeatMapEndTime()
    {
        beatMapEndTime = GetBeatMapEndTime();

        // If starting directly in the level scene
        if (StaticInformation.level == null)
        {
            Debug.Log("beatMapEndTime is set to its default of 10 seconds.");
        }

        // Check for incorrect input
        if (beatMapEndTime < 0 || beatMapEndTime > musicSource.clip.length)
        {
            Debug.Log("Invalid beatMapEndTime!");
        }
    }

    // This function is used when the beatMapEndTime is required at Start()
    public float GetBeatMapEndTime()
    {
        // Gets the current level, then references the scriptable object for beatMapEndTime
        string level = StaticInformation.level;

        if (level == null)
        {
            return 10f; // Default value
        }
        else
        {
            LevelDataScriptableObject scriptableObjectInstance = Resources.Load<LevelDataScriptableObject>($"LevelData\\{level}");
            beatMapEndTime = scriptableObjectInstance.beatMapEndTime;
        }
        return beatMapEndTime;
    }

    // [PRACTICE MODE] MAIN METHOD
    // Set the new time position and play
    public void SkipToTime(float timeSkipped)
    {
        LoadMusic(); // This method is called in PracticeManagerScript in Start(),
                     // hence, need to load the music before it is called later in this method,
                     // as it is not loaded in time in Start() in this script
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
