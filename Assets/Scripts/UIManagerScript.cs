using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerScript : MonoBehaviour
{
    public int health = 3;
    public Image[] hearts;
    public Sprite HeartEmpty;
    public Sprite HeartFull;

    public GameObject SceneManager;
    public GameObject AudioManager;
    public GameObject gameOverObject;
    public TMP_Text gameOverText;

    public float beatMapStartTime;

    private float audioTotalDuration;
    private float audioCompletedDuration;
    private float progressPercentage;

    // Start is called before the first frame update
    void Start()
    {
        audioTotalDuration = AudioManager.GetComponent<AudioManagerScript>().audioClip.length;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator GameOver()
    {   
        // Checks for the progress based on the length of the audio completed
        audioCompletedDuration = Time.time - beatMapStartTime; 
        progressPercentage = audioCompletedDuration / audioTotalDuration;
        gameOverText.text = "Game Over!" + Environment.NewLine + $"Progress:{progressPercentage.ToString("0.00")}%";
        gameOverObject.SetActive(true);

        // Pause for 5 seconds before restarting the scene
        yield return new WaitForSeconds(5);
        SceneManager.GetComponent<SceneManagerScript>().RestartScene();         
    }

    public void DecreaseHealth()
    {
        health--;

        // Change sprite of hearts based on health
        for (int heart = 0; heart < hearts.Count(); heart++)
        {
            if (heart < health)
            {
                hearts[heart].sprite = HeartFull;
            }
            else
            {
                hearts[heart].sprite = HeartEmpty;
            }
        }

        // End the game if no health is left
        if (health == 0)
        {
            StartCoroutine(GameOver());
        }
    }
}
