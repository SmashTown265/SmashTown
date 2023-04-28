using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LevelManager : NetworkBehaviour
{
	public static LevelManager instance;
	public Transform respawnPoint;
	public Transform player2Respawn;
	public Scene active;
    public GameObject character1;
    public GameObject character2;
    private GameObject player1Instance;
    private GameObject player2Instance;


	private void Start()
	{
		instance = this;
		active = SceneManager.GetActiveScene();
        print("Level Manager Awake");
        //spawn the players
        if (IsServer)
        {
            if(PlayerPortraitSwap.p1sprite == 0)
            {
                player1Instance = Instantiate(character1, respawnPoint.position, Quaternion.identity);
            }
            else
            {
                player1Instance = Instantiate(character2, respawnPoint.position, Quaternion.identity);
            }
            if(PlayerPortraitSwap.p2sprite == 0)
            {
                player2Instance = Instantiate(character1, player2Respawn.position, Quaternion.identity);
            }
            else
            {
                player2Instance = Instantiate(character2, player2Respawn.position, Quaternion.identity);
            }
            player1Instance.tag = "Player 1";
            player2Instance.tag = "Player 2";
            player1Instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.ConnectedClientsIds[0], true);
            player2Instance.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.ConnectedClientsIds[1], true);
        }
	}
    
	private void Awake()
	{
        print("Level Manager Start");
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
}
