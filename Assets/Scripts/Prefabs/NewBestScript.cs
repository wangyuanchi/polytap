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

    [Header("Particles")]
    [SerializeField] private ParticleSystem newBestParticles;

    [Header("SFX")]
    [SerializeField] private AudioClip newBestSFX;

    [Header("Canvas")]
    [SerializeField] private Canvas canvas;

    void Awake()
    {
        canvas.worldCamera = Camera.main;
        SetNewBestText();
        animator.SetTrigger("NewBest");
        LoadParticles();
        PlaySFX(newBestSFX);
        GameObject PostProcessing = GameObject.FindGameObjectWithTag("PostProcessing");
        StartCoroutine(PostProcessing.GetComponent<VignetteScript>().ChangeVignetteColor(new Color(0.15f, 1.00f, 0.44f))); // #25ff70
    }

    private void SetNewBestText()
    {
        string key = StaticInformation.level + "-" + PlayerPrefs.GetString("Mode") + "-HS";
        float newBestPercentage = PlayerPrefs.GetFloat(key);
        newBestText.text = $"NEW BEST!\n{newBestPercentage}%";
    }

    private void LoadParticles()
    {
        if (PlayerPrefs.GetString("Particles") == "true")
        { newBestParticles.Play(); }
    }

    private void PlaySFX(AudioClip SFX)
    {
        GetComponent<AudioSource>().clip = SFX;
        GetComponent<AudioSource>().Play();
    }
}
