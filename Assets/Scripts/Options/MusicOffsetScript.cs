using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicOffsetScript : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Slider musicOffsetSlider;
    [SerializeField] private TextMeshProUGUI numberText;

    // Start is called before the first frame update
    void Start()
    {
        LoadMusicOffset();
    }

    public void LoadMusicOffset()
    {
        numberText.text = PlayerPrefs.GetInt("Music Offset").ToString();
        musicOffsetSlider.value = PlayerPrefs.GetInt("Music Offset"); 
    }

    public void SetMusicOffset()
    {
        PlayerPrefs.SetInt("Music Offset", (int)musicOffsetSlider.value);
        numberText.text = ((int)musicOffsetSlider.value).ToString();
    }
}
