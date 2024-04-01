using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectorManagerScript : MonoBehaviour
{
    public void LoadLevel()
    { SceneManager.LoadSceneAsync("Level 1"); }
}
