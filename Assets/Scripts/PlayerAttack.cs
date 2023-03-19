using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

public class PlayerAttack : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 attackDir;
    Vector2 prevDir;
    float attackType;
    bool isAttacking;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        attackDir = GetDir(context.ReadValue<Vector3>());
        attackType = context.ReadValue<Vector3>().z;
        if (context.performed && attackType != 0)
        {
            isAttacking = true;
            print(attackDir);
        }
        if (context.canceled)
        {
            isAttacking = false;
        }
        
        
    }

    private Vector2 GetDir(Vector2 attackDir)
    {
        float x = Mathf.Abs(attackDir.x);
        float y = Mathf.Abs(attackDir.y);
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
        
    }

}
