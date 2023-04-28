using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour 
{


    [SerializeField] private TMP_Text messageText;



    private void Awake() 
    {

    }

    private void Start() 
    {
        NetworkRelay.Instance.OnFailedToJoinGame += Multiplayer_OnFailedToJoinGame;
        NetworkRelay.Instance.OnJoinStarted += OnJoinStarted;

        NetworkRelay.Instance.OnCreateGameFailed += OnCreateGameFailed;
        NetworkRelay.Instance.OnCreateGameStarted += OnCreateGameStarted;
        NetworkRelay.Instance.OnCreateGameSuccess += OnCreateGameSuccess;
        NetworkRelay.Instance.OnPlayerConnected += OnPlayerConnecting;
    }




    private void OnJoinStarted(object sender, System.EventArgs e) 
    {
        ShowMessage("Joining Game...");
    }

    private void OnCreateGameFailed(object sender, System.EventArgs e) 
    {
        ShowMessage("Failed to create Game!");
    }

    private void OnCreateGameStarted(object sender, System.EventArgs e) 
    {
        ShowMessage("Creating Game...");
    }

    private void Multiplayer_OnFailedToJoinGame(object sender, System.EventArgs e) 
    {
        if (NetworkManager.Singleton.DisconnectReason == "") 
        {
            ShowMessage("Failed to connect");
        } 
        else 
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void OnCreateGameSuccess(object sender, System.EventArgs e)
    {
        ShowMessage("Game Created...Waiting for Player 2");
    }

    private void OnPlayerConnecting(object sender, System.EventArgs e)
    {
        ShowMessage("Player 2 Joining...");
        StartCoroutine(GameStartedCoroutine());
    }

    private void ShowMessage(string message) 
    {
        messageText.text = message;
    }
    IEnumerator GameStartedCoroutine()
    {
        yield return new WaitForSeconds(2);
        StageManager.Instance.GoToNextScene();
    }

    private void OnDestroy() 
    {
        NetworkRelay.Instance.OnFailedToJoinGame -= Multiplayer_OnFailedToJoinGame;
        NetworkRelay.Instance.OnJoinStarted -= OnJoinStarted;

        NetworkRelay.Instance.OnCreateGameFailed -= OnCreateGameFailed;
        NetworkRelay.Instance.OnCreateGameStarted -= OnCreateGameStarted;
    }

}