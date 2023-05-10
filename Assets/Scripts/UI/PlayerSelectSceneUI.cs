using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class PlayerSelectSceneUI : MonoBehaviour
{

    [SerializeField] private Button continueButton;

    private void Awake()
    {
        // If online and not the server, hide the button, else listen for it
        if (!NetworkManager.Singleton.IsServer && NetworkRelay.Instance.online)
            continueButton.enabled = false;
        else
            continueButton.onClick.AddListener(() => {StageManager.Instance.GoToNextScene();});
    }
}
