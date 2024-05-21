using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DebugLogScript : MonoBehaviour
{
    public GameObject debugLog;
    public TMP_Text debugLogText;
    //string to store debug log text
    string debugLogMisses = "";

    public void AddText(string text)
    {
        debugLog.SetActive(true);
        //If first miss, changes text to received text. If not, write new line for text and append it.
        if (debugLogMisses == ""){debugLogMisses=text;}
        else {debugLogMisses += "<br>" + text;}
        debugLogText.text = debugLogMisses;
    }
}
