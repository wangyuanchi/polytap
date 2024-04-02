using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void LoadScene(string sceneName)
    { SceneManager.LoadSceneAsync(sceneName); }

    public void QuitGame()
    { Application.Quit(); }
}
