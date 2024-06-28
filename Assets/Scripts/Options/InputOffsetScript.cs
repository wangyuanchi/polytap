using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputOffsetScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Slider inputOffsetSlider;
    [SerializeField] private TextMeshProUGUI numberText;

    // Start is called before the first frame update
    void Start()
    {
        LoadInputOffset();
    }

    public void LoadInputOffset()
    {
        numberText.text = PlayerPrefs.GetInt("Input Offset").ToString();
        inputOffsetSlider.value = PlayerPrefs.GetInt("Input Offset"); 
    }

    public void SetInputOffset()
    {
        PlayerPrefs.SetInt("Input Offset", (int)inputOffsetSlider.value);
        numberText.text = ((int)inputOffsetSlider.value).ToString();
    }
}
