using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewBestScript : MonoBehaviour
{
    [Header("Overlay")]
    [SerializeField] private TextMeshProUGUI newBestText;
    [SerializeField] private Animator animator;

    [Header("SFX")]
    [SerializeField] private AudioClip newBestSFX;

    void Awake()
    {
        SetNewBestText();
        animator.SetTrigger("NewBest");
        PlaySFX(newBestSFX);
    }

    private void SetNewBestText()
    {
        string key = SceneManager.GetActiveScene().name + "-" + PlayerPrefs.GetString("Mode") + "-HS";
        float newBestPercentage = PlayerPrefs.GetFloat(key);
        newBestText.text = $"NEW BEST!\n{newBestPercentage}%";
    }

    private void PlaySFX(AudioClip SFX)
    {
        GetComponent<AudioSource>().clip = SFX;
        GetComponent<AudioSource>().Play();
    }
}
