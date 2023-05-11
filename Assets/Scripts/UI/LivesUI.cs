using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesUI : MonoBehaviour
{

    [SerializeField] public Transform playerlives;
    [SerializeField] public Camera cam;
    [SerializeField] public Vector3 offset;
        
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(playerlives.position + offset);
       

        if (transform.position != pos)
               transform.position = pos;
    }
}
