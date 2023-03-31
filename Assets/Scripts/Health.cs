using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
	public float damagePercent = 0f;

	private Rigidbody2D rb;
	private Animator anim;

	private Vector2 knockBackDir = Vector2.zero;
	// Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    public void OnDamage(float damage, float knockBack)
    {
	    damagePercent += damage;
        anim.SetTrigger("Damaged");
	    print(knockBack);
	    if (damagePercent < 100)
	    {
            knockBackDir.x = knockBack + (damagePercent / 10f) * Mathf.Sign(knockBack);
            
	    }
	    else
	    {
		    knockBackDir.Set(knockBack + ((damagePercent / 10f) * Mathf.Sign(knockBack)), .3f + (damagePercent / 10f));
		    

	    }
	    
        rb.AddForce(knockBackDir, ForceMode2D.Impulse);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
