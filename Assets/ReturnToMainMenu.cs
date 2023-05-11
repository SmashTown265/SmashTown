using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    bool online;
    public void Start()
    {
        if(NetworkRelay.Instance != null)
            online = NetworkRelay.Instance.online;
        else
            online = false;
    }
    public void OnReturn(InputAction.CallbackContext context)
    {
        if (online)
        {
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("mainMenuScene", LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("mainMenuScene", LoadSceneMode.Single);
        }
    }
    public void Update()
    {
       
    }
}
