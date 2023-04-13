
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

	public void GoToNextScene(int selection = 0)
	{
		var sceneName = SceneManager.GetActiveScene().name;

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

			case var _ when sceneName.Contains("Arena"):
			{
				switch (selection)
				{
					case 1:
						LoadScene("PlayerSelectScene");
						break;
					case 2:
						LoadScene("StageSelectScene");
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
		NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
	}
}