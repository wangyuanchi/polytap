using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectorManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject L1NProgressBarFilled;
    [SerializeField] private GameObject L1HProgressBarFilled;

    // Start is called before the first frame update
    void Start()
    {
        L1NProgressBarFilled.GetComponent<Image>().fillAmount = PlayerPrefs.GetFloat("L1-N-HS") / 100;
        L1HProgressBarFilled.GetComponent<Image>().fillAmount = PlayerPrefs.GetFloat("L1-H-HS") / 100;
    }

    public void LoadScene(string sceneName)
    { 
        SceneManager.LoadSceneAsync(sceneName); 
    }
}

