using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PracticeSkipDurationSliderScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Slider practiceSkipDurationSlider;
    [SerializeField] private TextMeshProUGUI numberText;

    // Start is called before the first frame update
    void Start()
    {
        LoadPracticeSkipDuration();
    }

    private void LoadPracticeSkipDuration()
    {
        numberText.text = Math.Round(PlayerPrefs.GetFloat("Practice Skip Duration"), 1).ToString(); // Round to 1 decimal places
        practiceSkipDurationSlider.value = PlayerPrefs.GetFloat("Practice Skip Duration");
    }

    public void SetPracticeSkipDuration()
    {
        PlayerPrefs.SetFloat("Practice Skip Duration", practiceSkipDurationSlider.value);
        numberText.text = Math.Round(practiceSkipDurationSlider.value, 1).ToString(); // Round to 1 decimal places
    }
}
