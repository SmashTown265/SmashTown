using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

	public void Awake()
	{
		// If there is already an instance of this object running
		// destroy the new one and keep the old one
		if(Instance  == null)
			Instance = this;
		else if(Instance != this)
			Destroy(this);
		// Keep instance of this gameObject running when loading new scenes
		DontDestroyOnLoad(gameObject);
	}
    // TODO: change case 2 loadscene when adding stage selection
	public void GoToNextScene(int selection = 0)
	{
		var sceneName = SceneManager.GetActiveScene().name;
        print(sceneName);
		switch (sceneName)
		{
			case "playerLobby":
				LoadScene("PlayerSelectScene");
				break;

			case "PlayerSelectScene":
				LoadScene("SwordFighterArena");
				break;

			case "StageSelectScene":
			{
				switch (selection)
				{
					case 1:
						LoadScene("SwordFighterArena");
						break;
					case 2:
						// Change this name to the second character stage
						LoadScene("ZonerArena");
						break;
					case 3:
						// Change this name to the third character stage
						LoadScene("ChangeThisLater");
						break;
				}
				break;
			}

			case var _ when sceneName.Contains("Arena") || sceneName.Contains("WinLose"):
			{
				switch (selection)
				{
                    case 0:
                        LoadScene("WinLose");
                        break;
					case 1:
                        // Change this to stage select when another stage is added
						LoadScene("SwordFighterArena");
						break;
					case 2:
						LoadScene("PlayerSelectScene");
						break;
					case 3:
						LoadScene(sceneName);
						break;
				}
				break;
			}
		}
	}
	

	private static void LoadScene(string sceneName)
	{
        LoadSceneMode load = LoadSceneMode.Single;
        if(sceneName == "WinLose")
            load = LoadSceneMode.Additive;
        else
            load = LoadSceneMode.Single;
        if(NetworkRelay.Instance.online)
		    NetworkManager.Singleton.SceneManager.LoadScene(sceneName, load);
        else
            SceneManager.LoadScene(sceneName, load);
	}
}