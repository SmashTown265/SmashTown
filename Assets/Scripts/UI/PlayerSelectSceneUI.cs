using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSelectSceneUI : MonoBehaviour
{

    [SerializeField] private Button continueButton;

    private void Awake()
    {
        continueButton.onClick.AddListener(() =>
        {
            StageManager.Instance.GoToNextScene();
        });
    }
}
