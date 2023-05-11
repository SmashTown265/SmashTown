using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FirstSelectedOnInput : MonoBehaviour
{

    private InputSystemUIInputModule UIinput;
    private GameObject firstSel;
    private InputAction navigation;
    private bool input = false;
    private void Start()
    {
        UIinput = gameObject.GetComponent<InputSystemUIInputModule>();
        navigation = UIinput.actionsAsset.FindAction("Navigate");
        firstSel = EventSystem.current.firstSelectedGameObject;
        SceneManager.activeSceneChanged += SceneLoaded;
    }
    private void Update()
    {
        if(!input)
            EventSystem.current.SetSelectedGameObject(null);
        if(navigation.phase != InputActionPhase.Waiting && (!input || EventSystem.current.currentSelectedGameObject == null))
        {
            EventSystem.current.SetSelectedGameObject(firstSel);
            input = true;
        }
    }
    private void SceneLoaded(Scene current, Scene next)
    {
        input = false;
        firstSel = EventSystem.current.firstSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
    }


    
}
