    using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class LogsScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private GameObject logsUI;
    [SerializeField] private TMP_Text logsText;
    private string tempText = "";

    public void AddLog(string text)
    {
        // If this function is ever called, logs would be set to true in playerprefs, hence set UI to actives
        logsUI.SetActive(true);

        // On first log, changes text to received text
        // Otherwise, append it as a new line
        if (tempText == "") { tempText = text; }
        else { tempText += "\n" + text; }
        logsText.text = tempText;
    }
}
