using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.Netcode;
using UnityEngine;

public class PlayerPortraitSwap : MonoBehaviour
{
	public GameObject p1l, p1r, p2l, p2r;
    private int p1sprite, p2sprite = 0;

    public Sprite[] sprite = new Sprite[3];
    public GameObject p1Portrait;
	public GameObject p2Portrait;

    // Start is called before the first frame update
    void Awake()
    {
	    if (NetworkManager.Singleton.IsHost)
	    {
		    p2l.SetActive(false);
		    p2r.SetActive(false);
	    }
	    else if (NetworkManager.Singleton.IsClient)
	    {
		    p1l.SetActive(false);
		    p1r.SetActive(false);
	    }
    }



    public void PlayerSwapLeft()
    {
	    if (NetworkManager.Singleton.IsHost)
	    {
		    p1sprite = p1sprite > 0 ? --p1sprite : 2;
		    p1Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p1sprite];
		    PortraitClientRpc(p1sprite);
	    }
		    
        else if (NetworkManager.Singleton.IsClient)
	    {
		    p2sprite = p2sprite > 0 ? --p2sprite : 2;
		    p2Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p2sprite];
		    PortraitServerRpc(p2sprite);
	    }
    }

    public void PlayerSwapRight()
    {
	    if (NetworkManager.Singleton.IsHost)
	    {
		    p1sprite = p1sprite < 2 ? ++p1sprite : 0;
		    p1Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p1sprite];
		    PortraitClientRpc(p1sprite);
	    }
		    
	    else if (NetworkManager.Singleton.IsClient)
	    {
		    p2sprite = p2sprite < 2 ? ++p2sprite : 0;
		    p2Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p2sprite];
		    PortraitServerRpc(p2sprite);
	    }
    }


    [ServerRpc]
    public void PortraitServerRpc(int val)
    {
	    p2Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[val];
    }
    [ClientRpc]
    public void PortraitClientRpc(int val)
    {
	    p1Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[val];
    }
}