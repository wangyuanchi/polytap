using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageVignetteToggleScript : MonoBehaviour
{
    [SerializeField] private Toggle damageVignetteToggle;

    // Start is called before the first frame update
    void Start()
    {
        LoadToggle();
    }

    private void LoadToggle()
    {
        if (PlayerPrefs.GetString("Damage Vignette") == "true")
        {
            damageVignetteToggle.isOn = true;
        }
        else
        {
            damageVignetteToggle.isOn = false;
        }
    }

    public void onToggle()
    {
        if (damageVignetteToggle.isOn)
        {
            PlayerPrefs.SetString("Damage Vignette", "true");
        }
        else
        {
            PlayerPrefs.SetString("Damage Vignette", "false");
        }
    }
}
