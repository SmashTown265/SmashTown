using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class WinScreenUpdate : NetworkBehaviour
{
    public TMP_Text winText;
    public bool player2PlayAgain = false;
    // Start is called before the first frame update
    void Start()
    {
        string text = "";
        if(!NetworkRelay.Instance.online)
            text = LevelManager.instance.player1Text;
        else
        {
            if(IsServer)
                text = LevelManager.instance.player1Text;
            else if(!IsServer)
                text = LevelManager.instance.player2Text;
        }
        winText.text = text;
    }
    [ServerRpc(RequireOwnership = false)]
    public void PlayAgainServerRpc()
    {
        if(IsServer)
        {
            player2PlayAgain = true;
            GameObject.Find("Other Player Text").SetActive(true);
        }
    }
    [ClientRpc]
    public void PlayAgainClientRpc()
    {
        if(IsServer)
        {
            GameObject.Find("Other Player Text").SetActive(true);
            GameObject.Find("Other Player Text").GetComponent<TMP_Text>().text = "Waiting For Other Player";
        }
        else
        {
            GameObject.Find("Other Player Text").SetActive(true);
            GameObject.Find("Other Player Text").GetComponent<TMP_Text>().text = "Player 1 Wants To Play Again!";
        }

    }
    public void PlayAgain()
    {
        if(!NetworkRelay.Instance.online)
            StageManager.Instance.GoToNextScene(1);
        else
        {
            if(IsServer && player2PlayAgain)
                StageManager.Instance.GoToNextScene(2);
            else if(!IsServer)
                PlayAgainServerRpc();
        }
        StartCoroutine(LevelManager.instance.LevelStartSoundCoroutine());
    }

}
