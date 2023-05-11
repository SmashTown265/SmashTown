using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.InputSystem;
using Unity.Netcode.Components;


public class LevelManager : NetworkBehaviour
{
	public static LevelManager instance;
	public Transform respawnPoint;
	public Transform player2Respawn;
	public Scene active;
    private bool online;
    public GameObject sf1;
    public GameObject z1;
    public GameObject sf2;
    public GameObject z2;
    private GameObject player1Instance;
    private GameObject player2Instance;
    public Camera2D cam2d;
    private List<Target2D> targets = new List<Target2D>();
    public GameObject inputManager;
    private SpriteRenderer Player1LifeCounter;
    private SpriteRenderer Player2LifeCounter;
    public int DeathCounterPlayer1 = 0;
    public int DeathCounterPlayer2 = 0;
    public Sprite lifezero;
    public Sprite lifetwo;
    public Sprite lifeone;
    [HideInInspector]
    public string player1Text;
    [HideInInspector]
    public string player2Text;

    
    private void Start()
	{
		instance = this;
		active = SceneManager.GetActiveScene();
        online = NetworkRelay.Instance.online;
        StartCoroutine(LevelStartSoundCoroutine());
        GameObject.Find("NetworkManager").GetComponent<PlayerInput>().enabled = false;
        //spawn the players
        if (IsServer || !online)
        {
            if(PlayerPortraitSwap.p1sprite == 0)
            {
                player1Instance = Instantiate(sf1, respawnPoint.position, Quaternion.identity);
            }
            else
            {
                player1Instance = Instantiate(z1, respawnPoint.position, Quaternion.identity);
            }
            if(PlayerPortraitSwap.p2sprite == 0)
            {
                player2Instance = Instantiate(sf2, player2Respawn.position, Quaternion.identity);
            }
            else
            {
                player2Instance = Instantiate(z2, player2Respawn.position, Quaternion.identity);
            }
            
            
            
            if (online)
            {
                // Spawn players
                player1Instance.GetComponent<NetworkObject>().SpawnWithOwnership(0, true);
                player2Instance.GetComponent<NetworkObject>().SpawnWithOwnership(1, true);
                
                // Enable death scripts on both host and client through RPC
                EnableDeathClientRpc();

            }
            else
            {
                
                // Enable Input Manager
                inputManager.GetComponent<PlayerInputManager>().enabled = true;

                // Disable netcode only conponents if local
                player1Instance.GetComponent<OwnerNetworkAnimator>().enabled = false;
                player2Instance.GetComponent<OwnerNetworkAnimator>().enabled = false;
                player1Instance.GetComponent<ImprovedNetworkRigidbody2D>().enabled = false;
                player2Instance.GetComponent<ImprovedNetworkRigidbody2D>().enabled = false;
                player1Instance.GetComponent<ImprovedNetworkTransform>().enabled = false;
                player2Instance.GetComponent<ImprovedNetworkTransform>().enabled = false;

                // Enable Death Scripts
                player1Instance.GetComponent<PlayerDeath>().enabled = true;
                player2Instance.GetComponent<PlayerDeath>().enabled = true;
            }
            // Add players to 2D camera targets
            targets.Add(new Target2D(player1Instance, false));
            targets.Add(new Target2D(player2Instance, false));
            cam2d.AddTargets(targets);
            Player1LifeCounter = GameObject.Find("Player1 Lives").GetComponentInChildren<SpriteRenderer>();
            Player2LifeCounter = GameObject.Find("Player2 Lives").GetComponentInChildren<SpriteRenderer>();
            
        }
	}
    
    [ServerRpc(RequireOwnership = false)] 
    public void DeathCounterServerRPC(ServerRpcParams rpcParams = default)
    {
        switch (DeathCounterPlayer1)
        {
            case 0:
                break;
            case 1:
                Player1LifeCounter.sprite = lifetwo;
                break;
            case 2: 
                Player1LifeCounter.sprite = lifeone;
                break;
            case 3:
                Player1LifeCounter.sprite = lifezero;
                break; // Put Death Screen Loss transition here and Winner for Player 2
        }
        switch (DeathCounterPlayer2)
        {
            case 0:
                break;
            case 1:
                Player2LifeCounter.sprite = lifetwo;
                break;
            case 2:
                Player2LifeCounter.sprite = lifeone;
                break;
            case 3:
                Player2LifeCounter.sprite = lifezero;
                break; // Put Death Screen Loss transition here and Winner for Player 2
        }

    }


    [ClientRpc]
    private void EnableDeathClientRpc( ClientRpcParams clientRpcParams = default) 
    {
        EnablePlayerDeathScript();
    }
	
	public void Respawn(GameObject playerObject, string tag)
	{
		if (tag == "Player 1")
		{
			StartCoroutine(RespawnCoroutine(playerObject, respawnPoint));
            DeathCounterPlayer1++;
		}

		if (tag == "Player 2")
		{
			StartCoroutine(RespawnCoroutine(playerObject, player2Respawn));
            DeathCounterPlayer2++;
		}
        UpdateLives();
		UnityEngine.Debug.Log("Player Respawned");
	}

    public IEnumerator RespawnCoroutine(GameObject playerObject, Transform respawn)
	{
        print("respawn");
        playerObject.SetActive(false);
		yield return new WaitForSeconds(.2f);
		playerObject.transform.position = respawn.position;
        playerObject.transform.localScale = respawn.localScale;
        playerObject.SetActive(true);
	}

    public void EnablePlayerDeathScript()
    {
            GameObject.FindWithTag("Player 1").GetComponent<PlayerDeath>().enabled = true;
            GameObject.FindWithTag("Player 2").GetComponent<PlayerDeath>().enabled = true;

    }

    public void UpdateLives()
    {
        print(DeathCounterPlayer1);
        print(DeathCounterPlayer2);
         switch (DeathCounterPlayer1)
        {
            case 0:
                break;
            case 1:
                Player1LifeCounter.sprite = lifetwo;
                break;
            case 2: 
                Player1LifeCounter.sprite = lifeone;
                break;
            case 3:
                Player1LifeCounter.sprite = lifezero;
                WinLose();
                break; // Put Death Screen Loss transition here and Winner for Player 2
        }
        switch (DeathCounterPlayer2)
        {
            case 0:
                break;
            case 1:
                Player2LifeCounter.sprite = lifetwo;
                break;
            case 2:
                Player2LifeCounter.sprite = lifeone;
                break;
            case 3:
                Player2LifeCounter.sprite = lifezero;
                WinLose();
                break; // Put Death Screen Loss transition here and Winner for Player 2
        }
    }
    public IEnumerator LevelEndSoundCoroutine()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Stop("Battle");
            AudioManager.instance.Play("WinStart");
            yield return new WaitForSeconds(2f);
            AudioManager.instance.Play("WinSound");
        }
    }
    public IEnumerator LevelStartSoundCoroutine()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.Stop("WinSound");
            AudioManager.instance.Stop("Theme");
            AudioManager.instance.Play("BattleStart");
            yield return new WaitForSeconds(1.5f);
            AudioManager.instance.Play("Battle");
        }
    }
    public void WinLose()
    {
        StartCoroutine(LevelEndSoundCoroutine());
        StageManager.Instance.GoToNextScene(0);
        
        if(!online)
        {
            if(DeathCounterPlayer1 < DeathCounterPlayer2)
                player1Text= "Player 1 Wins!";
            else
                player1Text= "Player 2 Wins!";
            
        }
        else
        {
            if(DeathCounterPlayer1 < DeathCounterPlayer2)
            {
                player1Text= "You Wins";
                player2Text = "You Lose";
            }
            else
            {
                player2Text = "You Wins";
                player1Text= "You Lose";
            }

        }
    }

}
