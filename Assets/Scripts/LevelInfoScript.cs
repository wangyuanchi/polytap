using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LevelInfoScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelInfoText;

    // Start is called before the first frame update
    void Awake()
    {
        string mode = PlayerPrefs.GetString("Mode") == "N" ? "Normal" : "Hard";
        string attemptsKey = SceneManager.GetActiveScene().name + "-" + PlayerPrefs.GetString("Mode") + "-TA";
        string attempts = PlayerPrefs.GetInt(attemptsKey).ToString();
        levelInfoText.text = $"Mode: {mode} \t Attempts: {attempts}";
    }
}
