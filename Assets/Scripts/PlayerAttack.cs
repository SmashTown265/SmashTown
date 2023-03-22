using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;
using UnityEngine.InputSystem.Interactions;

public class PlayerAttack : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    PlayerInput pI;
    PlayerMovement pM;
    Vector2 attackDir;
    Vector2 prevDir;
    float attackType;
    int attackCounter = 0;
    // Keep the isAttacking_ bool from being changed in other scripts
    public bool isAttacking => isAttacking_;
    [SerializeField] bool isAttacking_ = false;
    // public bool attackHeld;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        pI = GetComponent<PlayerInput>();
        pM = GetComponent<PlayerMovement>();

    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        // Attack only if not attacking and not dashing
        if (context.performed && !isAttacking_ && !pM.isDashing_)
        {
            attackDir = GetDir();
            isAttacking_ = true;
            anim.SetTrigger("attackTrigger");
            attackCounter = 0;
        }

    }

    private Vector2 GetDir()
    {
        // PlayerInput "move" action tracks left stick
        // so take it from that
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
        anim.SetInteger("AttackX", (int)attackDir.x);
        anim.SetInteger("AttackY", (int)attackDir.y);
        anim.SetBool("isAttacking", isAttacking_);
        if (attackCounter++ > 24)
        {
            isAttacking_ = false;
        }
        
    }

}
