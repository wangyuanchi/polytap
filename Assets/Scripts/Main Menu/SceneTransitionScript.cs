using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionScript : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private string levelToLoad;

    public static SceneTransitionScript instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void TransitionToScene(string levelName)
    {
        levelToLoad = levelName;
        animator.SetTrigger("FadeOut");
    }

    public void onFadeComplete()
    {
        SceneManager.LoadSceneAsync(levelToLoad);
        Time.timeScale = 1; // Pause causes timeScale to be 0, reset to 1 after new scene
        animator.SetTrigger("FadeIn");
    }
}
