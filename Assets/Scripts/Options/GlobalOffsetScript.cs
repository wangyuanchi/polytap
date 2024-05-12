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

    // Update is called once per frame
    private void LoadGlobalOffset()
    {
        numberText.text = PlayerPrefs.GetInt("GlobalOffset").ToString() + "ms";
        globalOffsetSlider.value = PlayerPrefs.GetInt("GlobalOffset");
    }

    public void SetGlobalOffset()
    {
        PlayerPrefs.SetInt("GlobalOffset", (int)globalOffsetSlider.value * 5); //the * 5 is so that it shifts by steps of 5
        numberText.text = ((int)globalOffsetSlider.value * 5).ToString() + "ms";
    }
}
