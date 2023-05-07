using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class OwnerNetworkAnimator : NetworkBehaviour {
    private PlayerMovement player1;
    private PlayerMovement player2;
    private void Start()
    {
        player1 = GameObject.FindWithTag("Player 1").GetComponent<PlayerMovement>();
        player2 = GameObject.FindWithTag("Player 2").GetComponent<PlayerMovement>();
    }
    private void FixedUpdate()
    {
        if (IsServer)
        {
            PlayerStateClientRpc(player1.playerState);
        }
        if (IsClient)
        {
            PlayerStateServerRpc(player2.playerState);
        }
    }
    [ClientRpc]
    void PlayerStateClientRpc(State playerState)
    {
        if(!IsServer)
            player1.playerState = playerState;
    }
    [ServerRpc(RequireOwnership = false)]
    void PlayerStateServerRpc(State playerState)
    {
        player2.playerState = playerState;
    }
}