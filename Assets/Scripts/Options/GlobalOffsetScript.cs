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

    public void LoadGlobalOffset()
    {
        numberText.text = PlayerPrefs.GetInt("Global Offset").ToString();
        globalOffsetSlider.value = PlayerPrefs.GetInt("Global Offset"); 
    }

    public void SetGlobalOffset()
    {
        PlayerPrefs.SetInt("Global Offset", (int)globalOffsetSlider.value);
        numberText.text = ((int)globalOffsetSlider.value).ToString();
    }
}
