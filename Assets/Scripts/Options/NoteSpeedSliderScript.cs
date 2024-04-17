using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NoteSpeedSliderScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Slider noteSpeedSlider;
    [SerializeField] private TextMeshProUGUI numberText;

    // Start is called before the first frame update
    void Start()
    {
        LoadNoteSpeed();
    }

    private void LoadNoteSpeed()
    {
        numberText.text = PlayerPrefs.GetInt("Note Speed").ToString();
        noteSpeedSlider.value = PlayerPrefs.GetInt("Note Speed");
    }

    public void SetNoteSpeed()
    {
        PlayerPrefs.SetInt("Note Speed", (int)noteSpeedSlider.value);
        numberText.text = ((int)noteSpeedSlider.value).ToString();
    }    
}
