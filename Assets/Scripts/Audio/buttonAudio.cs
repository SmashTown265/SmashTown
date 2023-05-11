using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class buttonAudio : MonoBehaviour, IPointerEnterHandler
{

    public Button[] buttons;
    private AudioManager audioManager;
    EventTrigger.Entry entry = new EventTrigger.Entry();
    EventTrigger.Entry entry2 = new EventTrigger.Entry();
         
    
         

// Start is called before the first frame update
    public void Start()
    {
     
        audioManager = FindFirstObjectByType<AudioManager>();
        AddButtons();
        SceneManager.activeSceneChanged += SceneLoaded;
    }

    private void SceneLoaded(Scene current, Scene next)
    {
        System.Array.Clear(buttons, 0, buttons.Length);
        AddButtons();
    }
    private void AddButtons()
    {
        // Just Don't Look at it. 
        buttons = FindObjectsOfType<Button>(true);
        foreach (var button in buttons)
        {   
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((data) => { OnPointerSelect((PointerEventData) data); });
            if(button.gameObject.GetComponent<EventTrigger>() != null)
            {
                if(!button.gameObject.GetComponent<EventTrigger>().triggers.Contains(entry))
                    button.gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
            }
            else
            {
                print("Should be adding componenets...");
                button.gameObject.AddComponent<EventTrigger>();
                button.gameObject.GetComponent<EventTrigger>().triggers.Add(entry);
            }
            entry2.eventID = EventTriggerType.PointerEnter;
            entry2.callback.AddListener((data) => { OnPointerEnter((PointerEventData) data); });
            if(!button.gameObject.GetComponent<EventTrigger>().triggers.Contains(entry2))
                button.gameObject.GetComponent<EventTrigger>().triggers.Add(entry2);
        }
    }
    public void buttonHover()
    {
        audioManager.Play("button hover");
    }
    public void buttonClick()
    {
        audioManager.Play("button click");
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonHover();
    }
    public void OnPointerSelect(PointerEventData eventData)
    {
        buttonClick();
    }

}
