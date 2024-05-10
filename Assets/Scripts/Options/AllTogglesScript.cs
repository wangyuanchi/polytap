using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AllTogglesScript : MonoBehaviour
{
    [SerializeField] private Toggle vignetteToggle;
    [SerializeField] private Toggle particlesToggle;
    [SerializeField] private Toggle timingToggle;

    private Dictionary<string, Toggle> prefToggleDict = new Dictionary<string, Toggle>();

    // Start is called before the first frame update
    void Start()
    {
        // Make sure that the key is the same as PlayerPrefs and in the onValueChanged function of each toggle
        prefToggleDict.Add("Vignette", vignetteToggle);
        prefToggleDict.Add("Particles", particlesToggle);
        prefToggleDict.Add("ShowTiming", timingToggle);
        LoadAllToggles();
    }

    private void LoadToggle(string pref)
    {
        if (PlayerPrefs.GetString(pref) == "true")
        {
            prefToggleDict[pref].isOn = true;
        }
        else
        {
            prefToggleDict[pref].isOn = false;
        }
    }

    private void LoadAllToggles()
    {
        foreach (KeyValuePair<string, Toggle> kvp in prefToggleDict)
        {
            LoadToggle(kvp.Key);
        }
    }

    public void onToggle(string pref)
    {
        if (prefToggleDict[pref].isOn)
        {
            PlayerPrefs.SetString(pref, "true");
        }
        else
        {
            PlayerPrefs.SetString(pref, "false");
        }
    }
}
