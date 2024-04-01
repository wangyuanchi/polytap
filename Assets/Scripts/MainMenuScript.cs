using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void LoadLevelSelector()
    { SceneManager.LoadSceneAsync("Level Selector"); }

    public void LoadOptions()
    { SceneManager.LoadSceneAsync("Options"); }

    public void QuitGame()
    { Application.Quit(); }
}
