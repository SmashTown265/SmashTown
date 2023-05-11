using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerPortraitSwap : NetworkBehaviour
{
	public GameObject p1l, p1r, p2l, p2r;
    public static int p1sprite, p2sprite = 0;
    // TODO: change this array to 3 when adding new character
    public Sprite[] sprite = new Sprite[2];
    public GameObject p1Portrait;
	public GameObject p2Portrait;
    private bool online;
    // Start is called before the first frame update
    void Awake()
    {
        online = GameObject.FindWithTag("NetworkManager").GetComponent<NetworkRelay>().online;
        if(online)
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
    }


    // TODO: change these from 1 to 2 when adding new character
    public void PlayerSwapLeft(int player)
    {
	    if (NetworkManager.Singleton.IsHost || (!online && player == 1))
	    {
		    p1sprite = p1sprite > 0 ? --p1sprite : 1;
		    p1Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p1sprite];
		    if (online)
                PortraitClientRpc(p1sprite);
	    }
		    
        else if (NetworkManager.Singleton.IsClient || (!online && player == 2))
	    {
		    p2sprite = p2sprite > 0 ? --p2sprite : 1;
		    p2Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p2sprite];
		    if (online)
                PortraitServerRpc(p2sprite);
	    }
    }

    public void PlayerSwapRight(int player)
    {
	    if (NetworkManager.Singleton.IsHost || (!online && player == 1))
	    {
		    p1sprite = p1sprite < 1 ? ++p1sprite : 0;
		    p1Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p1sprite];
		    if(online)
                PortraitClientRpc(p1sprite);
	    }
		    
	    else if (NetworkManager.Singleton.IsClient || (!online && player == 2))
	    {
		    p2sprite = p2sprite < 1 ? ++p2sprite : 0;
		    p2Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p2sprite];
            if(online)
		        PortraitServerRpc(p2sprite);
	    }
    }


    [ServerRpc(RequireOwnership = false)]
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
