using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class LevelManager : NetworkBehaviour
{
	public static LevelManager instance;
	public Transform respawnPoint;
	public Transform player2Respawn;
	public Scene active;

    public GameObject sf1;
    public GameObject z1;
    public GameObject sf2;
    public GameObject z2;
    private GameObject player1Instance;
    private GameObject player2Instance;


	private void Start()
	{
		instance = this;
		active = SceneManager.GetActiveScene();

        //spawn the players
        if (IsServer)
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

            player1Instance.GetComponent<NetworkObject>().SpawnWithOwnership(0, true);
            player2Instance.GetComponent<NetworkObject>().SpawnWithOwnership(1, true);
            EnableDeathClientRpc();
            // EnableMovement Moved to PlayerMovement Script OnNetworkSpawn()

        }
	}
    
	private void Awake()
	{
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
		}

		if (tag == "Player 2")
		{
			StartCoroutine(RespawnCoroutine(playerObject, player2Respawn));
		}
		UnityEngine.Debug.Log("Player Respawned");
	}

    public IEnumerator RespawnCoroutine(GameObject playerObject, Transform respawn)
	{
        playerObject.SetActive(false);
		yield return new WaitForSeconds(.2f);
		playerObject.transform.position = respawn.position;
        playerObject.transform.localScale = respawn.localScale;
        playerObject.SetActive(true);
	}

    public void EnablePlayerDeathScript()
    {
        if(IsServer)
        {
            GameObject.FindWithTag("Player 1").GetComponent<PlayerDeath>().enabled = true;
        }
        else if(!IsServer)
        {
            GameObject.FindWithTag("Player 2").GetComponent<PlayerDeath>().enabled = true;
        }
    }

}
