using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
	private BoxCollider2D bc2d;
	private void Awake()
	{
		bc2d = GetComponent<BoxCollider2D>();
	}
	private void OnEnable()
	{
		if (gameObject.CompareTag("Player 1"))
		{
			GameObject other = GameObject.FindWithTag("Player 2");
			BoxCollider2D p2Collider = other.GetComponent<BoxCollider2D>();
			Physics2D.IgnoreCollision(bc2d, p2Collider);
		}
        else if (gameObject.CompareTag("Player 2"))
		{
			GameObject other = GameObject.FindWithTag("Player 1");
			BoxCollider2D p2Collider = other.GetComponent<BoxCollider2D>();
			Physics2D.IgnoreCollision(bc2d, p2Collider);
        }
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DeathZone"))
        {
	        LevelManager.instance.Respawn(gameObject, gameObject.tag);
        }
    }
}
