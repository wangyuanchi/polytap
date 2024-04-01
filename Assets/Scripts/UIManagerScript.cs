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
    public GameObject NoteManager;
    public GameObject gameOverObject;
    public TMP_Text gameOverText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void GameOver()
    {   
        //Gets percentage of notes completed
        float note = NoteManager.GetComponent<NotesManagerScript>().currentNote;
        float max = NoteManager.GetComponent<NotesManagerScript>().beatMapLength;
        float progress = Mathf.Floor((note / max)*100);
        gameOverText.text = $"Game Over! Progress:{progress}%";
        gameOverObject.SetActive(true);
        StartCoroutine(waiter());           //Needed to let text be shown for a few seconds
    }

    IEnumerator waiter()
    {   
        //just waits for 5 seconds before restarting
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
            GameOver();
        }
    }
}
