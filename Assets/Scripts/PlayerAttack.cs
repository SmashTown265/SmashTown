using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
	private Animator anim;
	private PlayerInput pI;
	private PlayerMovement pM;
	private Vector2 attackDir;
	public float attackTimer = 0;


	// Start is called before the first frame update
	private void Start()
	{
		anim = GetComponent<Animator>();
		pI = GetComponent<PlayerInput>();
		pM = GetComponent<PlayerMovement>();
	}

	public void OnAttack(InputAction.CallbackContext context)
	{
		print("Button action");
		// Attack only if not attacking and not dashing
		if (context.performed && attackTimer == 0 && (pM.playerState != State.Dashing || !pM.playerState.HasFlags(State.InAir)))
		{
			attackDir = GetDir();
			pM.playerState.AddFlag(State.Attacking);
            print($"THIS SHOULD BE TRUE ATTACKING: {pM.playerState.HasFlags(State.Attacking)} ");
            anim.SetTrigger("attackTrigger");
		}
	}

	public Vector2 GetDir()
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
		if (attackTimer > .15 && pM.playerState.HasFlags(State.Attacking))
		{
			pM.playerState.RemoveFlag(State.Attacking);
			pM.movementVector.x = Mathf.Abs(pM.stickPos.x) > .05f ? pM.stickPos.x : 0f;

            print($"THIS SHOULD BE FALSE ATTACKING: {pM.playerState.HasFlags(State.Attacking)} ");
		}
        else if (attackTimer > 0 && attackTimer < .4 || pM.playerState.HasFlags(State.Attacking))
		{
			attackTimer += Time.deltaTime;
			if (attackTimer >= .4)
				attackTimer = 0;
		}
	}
}
