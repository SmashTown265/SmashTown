using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : MonoBehaviour
{
	public float attackDamage = 5f;
	private Transform t;
	public float knockBack = 3f;

	// Start is called before the first frame update
    void Start()
    {
	   t = GetComponentsInParent<Transform>(true)[1];
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
	    if (other.gameObject.CompareTag("Player 2"))
	    {
            if(knockBack * t.localScale.x < 0)
				knockBack *= -1;
		    print($"knockback {knockBack}");
		    var damage = other.gameObject.GetComponent<Health>();
		    damage.OnDamage(attackDamage, knockBack);
	    }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
