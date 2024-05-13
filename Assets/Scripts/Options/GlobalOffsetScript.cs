using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GlobalOffsetScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Slider globalOffsetSlider;
    [SerializeField] private TextMeshProUGUI numberText;

    // Start is called before the first frame update
    void Start()
    {
        LoadGlobalOffset();
    }

    private void LoadGlobalOffset()
    {
        numberText.text = PlayerPrefs.GetInt("Global Offset").ToString();
        globalOffsetSlider.value = PlayerPrefs.GetInt("Global Offset") / 5; // Need to convert back to normal slider divisions
    }

    public void SetGlobalOffset()
    {
        PlayerPrefs.SetInt("Global Offset", (int)globalOffsetSlider.value * 5); // Shifts by divisions of 5ms
        numberText.text = ((int)globalOffsetSlider.value * 5).ToString();
    }
}
