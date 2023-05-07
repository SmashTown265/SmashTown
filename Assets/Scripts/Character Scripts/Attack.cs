
using Unity.Netcode;
using UnityEngine;


public class Attack : NetworkBehaviour
{
	public float attackDamage = 5f;
	private Transform t;
	public float knockBack = 3f;
    private string playerTag;
    private Health player1;
    private Health player2;
	// Start is called before the first frame update
    void Start()
    {
        t = GetComponentsInParent<Transform>(true)[1];
        playerTag = gameObject.tag;
        player1 = GameObject.FindWithTag("Player 1").GetComponent<Health>();
        player2 = GameObject.FindWithTag("Player 2").GetComponent<Health>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.GetType() == typeof(BoxCollider2D))
            print("Testing colliderType");
	    if (!other.gameObject.CompareTag(playerTag))
	    {
            if(knockBack * t.localScale.x < 0)
				knockBack *= -1;
		    print($"knockback {knockBack}");
            var damage = other.gameObject.GetComponent<Health>();
		    damage.OnDamage(attackDamage, knockBack);
            if (IsServer)
                OnDamageClientRpc(knockBack, attackDamage);
            else if (!IsServer)
                OnDamageServerRpc(knockBack, attackDamage);
	    }
    }

        
    [ServerRpc(RequireOwnership = false)]
    private void OnDamageServerRpc(float knockBack, float attackDamage, ServerRpcParams serverRpcParams = default) 
    {
        player1.OnDamage(attackDamage, knockBack);
    }

    [ClientRpc]
    private void OnDamageClientRpc(float knockBack, float attackDamage,  ClientRpcParams clientRpcParams = default) 
    {
        if (!IsServer)
        {
            player2.OnDamage(attackDamage, knockBack);
        }
    }
}
