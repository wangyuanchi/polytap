using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorManagerScript : MonoBehaviour
{
    public TMP_Text level1HighScoreText;
    public TMP_Text level1HardModeHighScoreText;

    public GameObject level1HighScoreBar;
    public GameObject level1HardScoreBar;

    // Start is called before the first frame update
    void Start()
    {
        level1HighScoreText.text = $"High Score: {PlayerPrefs.GetFloat("Level 1 High Score")}%";
        level1HighScoreBar.transform.localScale = new Vector3(PlayerPrefs.GetFloat("Level 1 High Score"), 1, 1);
        level1HardModeHighScoreText.text = $"Hard Mode High Score: {PlayerPrefs.GetFloat("Level 1 Hard Mode High Score")}%";
        level1HardScoreBar.transform.localScale = new Vector3(PlayerPrefs.GetFloat("Level 1 Hard Mode High Score"), 1, 1);
    }

    public void LoadScene(string sceneName)
    { 
        SceneManager.LoadSceneAsync(sceneName); 
    }
}

