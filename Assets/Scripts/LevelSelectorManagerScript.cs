using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorManagerScript : MonoBehaviour
{
    public TMP_Text level1HighScoreText;

    // Start is called before the first frame update
    void Start()
    {
        level1HighScoreText.text = $"High Score: {PlayerPrefs.GetFloat("Level 1 High Score")}%";
    }

    public void LoadScene(string sceneName)
    { 
        SceneManager.LoadSceneAsync(sceneName); 
    }
}

