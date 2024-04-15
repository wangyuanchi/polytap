using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMusicToggleScript : MonoBehaviour
{
    [SerializeField] private Toggle lobbyMusicToggle;

    // Start is called before the first frame update
    void Start()
    {
        LoadToggle();
    }

    private void LoadToggle()
    {
        if (PlayerPrefs.GetString("Lobby Music") == "true")
        {
            lobbyMusicToggle.isOn = true;
        }
        else
        {
            lobbyMusicToggle.isOn = false;  
        }
    }

    public void onToggle()
    {
        if (lobbyMusicToggle.isOn)
        {
            PlayerPrefs.SetString("Lobby Music", "true");
            LobbyMusicScript.instance.PlayLobbyMusic();
        }    
        else
        {
            PlayerPrefs.SetString("Lobby Music", "false");
            LobbyMusicScript.instance.StopLobbyMusic();
        }
    }
}
