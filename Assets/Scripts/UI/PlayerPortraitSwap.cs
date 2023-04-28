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


    // TODO: change these from 1 to 2 when adding new character
    public void PlayerSwapLeft()
    {
	    if (NetworkManager.Singleton.IsHost)
	    {
		    p1sprite = p1sprite > 0 ? --p1sprite : 1;
		    p1Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p1sprite];
		    PortraitClientRpc(p1sprite);
	    }
		    
        else if (NetworkManager.Singleton.IsClient)
	    {
		    p2sprite = p2sprite > 0 ? --p2sprite : 1;
		    p2Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p2sprite];
		    PortraitServerRpc(p2sprite);
	    }
    }

    public void PlayerSwapRight()
    {
	    if (NetworkManager.Singleton.IsHost)
	    {
		    p1sprite = p1sprite < 1 ? ++p1sprite : 0;
		    p1Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p1sprite];
		    PortraitClientRpc(p1sprite);
	    }
		    
	    else if (NetworkManager.Singleton.IsClient)
	    {
		    p2sprite = p2sprite < 1 ? ++p2sprite : 0;
		    p2Portrait.GetComponent<UnityEngine.UI.Image>().sprite = sprite[p2sprite];
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
