using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeModeScript : MonoBehaviour
{
    [SerializeField] private TMP_Text currentModeText;

    // Start is called before the first frame update
    void Start()
    {
        LoadMode();
    }

    private void LoadMode()
    {
        if (PlayerPrefs.GetString("Mode") == "N")
        {
            currentModeText.text = "Normal Mode";
        }
        else if (PlayerPrefs.GetString("Mode") == "H")
        {
            currentModeText.text = "Hard Mode";
        }
        else if (PlayerPrefs.GetString("Mode") == "A")
        {
            currentModeText.text = "Accuracy Mode";
        }
        else
        {
            Debug.Log("Error in loading mode!");
        }
    }

    // Sequence of modes: Normal -> Hard -> Accuracy
    public void ChangeMode()
    {
        if (PlayerPrefs.GetString("Mode") == "N")
        {
            currentModeText.text = "Hard Mode";
            PlayerPrefs.SetString("Mode", "H");
        }
        else if (PlayerPrefs.GetString("Mode") == "H")
        {
            currentModeText.text = "Accuracy Mode";
            PlayerPrefs.SetString("Mode", "A");
        }
        else if (PlayerPrefs.GetString("Mode") == "A")
        {
            currentModeText.text = "Normal Mode";
            PlayerPrefs.SetString("Mode", "N");
        }
        else
        {
            Debug.Log("Error in changing mode!");
        }
    }
}
