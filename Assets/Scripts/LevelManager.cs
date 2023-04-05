using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
	public static LevelManager instance;
	public Transform respawnPoint;
	public Transform player2Respawn;
	public Scene active;

	private void Awake()
	{
		instance = this;
		active = SceneManager.GetActiveScene();
	}

	private void Start()
	{

	}

	
	public void Respawn(GameObject playerObject, string tag)
	{
		if (tag == "Player")
		{
			StartCoroutine(RespawnCoroutine(playerObject, respawnPoint));
		}

		if (tag == "Player 2")
		{
			StartCoroutine(RespawnCoroutine(playerObject, player2Respawn));
		}
		playerObject.transform.localScale = FlipX(playerObject.transform.localScale);
		UnityEngine.Debug.Log("Player Respawned");
	}

	private static Vector3 FlipX(Vector3 toFlip)
	{
		toFlip.x *= -1;
		return toFlip;
	}

public IEnumerator RespawnCoroutine(GameObject playerObject, Transform respawn)
	{
        playerObject.SetActive(false);
		yield return new WaitForSeconds(.2f);
		playerObject.transform.position = respawn.position;
        playerObject.SetActive(true);

	}
}
