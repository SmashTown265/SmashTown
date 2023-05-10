using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayLocal()
    {
	    SceneManager.LoadScene("PlayerSelectScene");
    }

    public void PlayOnline()
    {
	    SceneManager.LoadScene("playerLobby");
    }

    public void QuitGame ()
    {
        Debug.Log("Game has exited");
        Application.Quit();
    }
}
