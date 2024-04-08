using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectorManagerScript : MonoBehaviour
{
    [SerializeField] private GameObject allLevelsGameObject;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform level in allLevelsGameObject.transform)
        {
            // Remove this after all levels have been made
            if (level.name != "L1")
            {
                continue;
            }
            LoadProgressBars(level);
        }
    }

    private void LoadProgressBars(Transform level)
    {
        string levelName = level.name;
        float normalModeHighScore = PlayerPrefs.GetFloat($"{levelName}-N-HS");
        float hardModeHighScore = PlayerPrefs.GetFloat($"{levelName}-H-HS");

        GameObject levelGameObject = level.gameObject;
        GameObject normalModeProgressBar = levelGameObject.transform.Find("NormalModeProgressBar").gameObject;
        GameObject hardModeProgressBar = levelGameObject.transform.Find("HardModeProgressBar").gameObject;

        // Set progress bar fill
        normalModeProgressBar.transform.Find("ProgressBarFilled").GetComponent<Image>().fillAmount = normalModeHighScore / 100;
        hardModeProgressBar.transform.Find("ProgressBarFilled").GetComponent<Image>().fillAmount = hardModeHighScore / 100;

        // Set progress text
        normalModeProgressBar.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = normalModeHighScore.ToString() + "%";
        hardModeProgressBar.transform.Find("ProgressText").GetComponent<TextMeshProUGUI>().text = hardModeHighScore.ToString() + "%";
    }
}

