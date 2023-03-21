using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Interactions;

public class PlayerAttack : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    PlayerInput pI;
    Vector2 attackDir;
    Vector2 prevDir;
    float attackType;
    public bool isAttacking;
    public bool attackHeld;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pI = GetComponent<PlayerInput>();

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && isAttacking == false)
        {
            attackDir = GetDir();
            anim.SetInteger("AttackX", (int)attackDir.x);
            anim.SetInteger("AttackY", (int)attackDir.y);
            anim.SetTrigger("attackTrigger");
            isAttacking = true;
        }
        if (context.canceled)
        {
            isAttacking = false;
        }
        anim.SetBool("isAttacking", isAttacking);

    }

    private Vector2 GetDir()
    {
        // Player Movement already has current stick vector, so take it from that
        Vector2 attackDir = pI.actions["Move"].ReadValue<Vector2>();
        
        float x = Mathf.Abs(attackDir.x);
        float y = Mathf.Abs(attackDir.y);

        // Sets the Vector2 to a specific quadrant based on x and y magnitudes
        if (x > y)
            attackDir.Set(Mathf.Sign(attackDir.x), 0);
        else if (y > x)
            attackDir.Set(0, Mathf.Sign(attackDir.y));
        else
            attackDir.Set(0, 0);

        return attackDir;
    }

    // Update is called once per frame
    void Update()
    {
        // do I need this for anything? Probably in the future...
    }

}
