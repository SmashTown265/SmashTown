using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Interactions;

public class PlayerAttack : MonoBehaviour
{
	private Rigidbody2D rb;
	private Animator anim;
	private PlayerInput pI;
	private PlayerMovement pM;
	private Vector2 attackDir;
	private int attackCounter;


	// Start is called before the first frame update
	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		anim = GetComponent<Animator>();
		pI = GetComponent<PlayerInput>();
		pM = GetComponent<PlayerMovement>();
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		// Attack only if not attacking and not dashing
		if (context.performed && !pM.playerState.HasFlags(State.Attacking) && pM.playerState != State.Dashing)
		{
			attackDir = GetDir();
			pM.playerState.AddFlag(State.Attacking);
			anim.SetTrigger("attackTrigger");
			attackCounter = 0;
		}
	}

	private Vector2 GetDir()
	{
		// PlayerInput "move" action tracks left stick
		// so take it from that
		attackDir = pI.actions["Move"].ReadValue<Vector2>();

		var x = Mathf.Abs(attackDir.x);
		var y = Mathf.Abs(attackDir.y);

		// Sets the Vector2 to a specific quadrant based on x and y magnitudes
		if (x > y)
		{
			attackDir.Set(Mathf.Sign(attackDir.x), 0);
		}
		else if (y > x)
		{
			attackDir.Set(0, Mathf.Sign(attackDir.y));
		}
		else
		{
			attackDir.Set(0, 0);
		}

		return attackDir;
	}

	// Update is called once per frame
	private void Update()
	{
		anim.SetInteger("AttackX", (int)attackDir.x);
		anim.SetInteger("AttackY", (int)attackDir.y);
		anim.SetBool("isAttacking", pM.playerState.HasFlags(State.Attacking));
		if (attackCounter++ > 24)
		{
			pM.playerState.RemoveFlag(State.Attacking);
		}
	}
}
