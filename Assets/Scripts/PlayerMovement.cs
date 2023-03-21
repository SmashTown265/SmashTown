using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

// This script is designed to have the OnMove and
// OnJump methods called by a PlayerInput component

public class PlayerMovement : MonoBehaviour
{
    
    Rigidbody2D rb;
    Animator anim;
    Transform t;
    BoxCollider2D bc2d;
    SpriteRenderer sr;
    PlayerAttack pa;
    Vector3 scaleFlip = new(-1, 1, 1); 
    Vector2 jump = Vector2.up;
    Vector2 tempVector = Vector2.zero;
    int Xdir;
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float inAirMovementMultiplier;
    [SerializeField] float inAirMoveSpeed;
    [SerializeField] float decelSpeed;

    [Header("Bools")]
    public bool isGrounded = false;
    bool isMoving = false;
    public bool doubleJumping = false;

    [Header("Inputs")]
    public Vector2 movementVector;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 10f;
        jumpPower = 4.5f;
        inAirMovementMultiplier = .15f;
        inAirMoveSpeed = 6f;
        decelSpeed = 75f;
        // Cache components on start to avoid lag
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        t = GetComponent<Transform>();
        bc2d = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        pa = GetComponent<PlayerAttack>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        
        // context.performed is whenever the input changes to a non-zero value
        if (context.performed)
        {
            // Read the value for the "move" action each event call
            movementVector.x = context.ReadValue<Vector2>().x;
            if(Mathf.Abs(movementVector.x) > .98f)
               movementVector.x = Mathf.Sign(movementVector.x);
            if(Mathf.Abs(movementVector.x) > .1f)
                Xdir = (int)Mathf.Sign(movementVector.x);
               
            isMoving = true;
            // Change of values located in FixedUpdate() which is called on physics updates (50 per second)
        }
        // context.canceled is whenever the input goes back to default state, called once
        // has to be "else if" because there is also context.started
        else if (context.canceled) 
        {
            print("Canceled");
            // Set isMoving to false
            isMoving = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if ((isGrounded || !doubleJumping) && context.performed)
        {
            jump.y = jumpPower; // ! Remove before release
            tempVector.x = rb.velocity.x;
            tempVector.y = 0f;
            if (((int)(movementVector.x * 10) ^ (int)(rb.velocity.x * 10)) < 0)
                tempVector.x = 0f;
            rb.velocity = tempVector;
            rb.AddForce(jump, ForceMode2D.Impulse);
            doubleJumping = !isGrounded;
            isGrounded = false;
            
        }

    }
    void OnCollisionEnter2D(Collision2D other)
    {
        // When it comes in contact with the ground
        // TODO: add conditional to make sure it is the ground, and not the side of something else
        if (other.gameObject.tag is "Ground" or "Platform")
        {
            doubleJumping = false;
            isGrounded = true;
        }
    }
    void OnCollisionExit2D(Collision2D other)
    {
        // When it comes in contact with the ground
        // TODO: add conditional to make sure it is the ground, and not the side of something else
        if (other.gameObject.tag is "Ground" or "Platform")
        {
            isGrounded = false;
        }
    }

    public void FixedUpdate()
    {

        // If the player is not giving movement input, and is on the ground
        // change x velocity to 0
        
        if (isGrounded)
        {
            // If grounded, set movement velocity to
            // the direction of movement multiplied by the default movement speed
            movementVector.x *= moveSpeed;
            rb.velocity = movementVector;
            movementVector.x /= moveSpeed;

            if (!isMoving || (pa.isAttacking && !pa.attackHeld))
            {
                anim.SetTrigger("stopAttack");
                rb.velocity = Vector2.zero;
            }

        }
        // If in the air, and velocity + anticipated movement is less than set movement speed
        // change the current velocity by direction multiplied
        // by the default in air movement speed multiplier

        else 
        {
            movementVector.x *= inAirMovementMultiplier;
            if (Mathf.Abs(rb.velocity.x) > inAirMoveSpeed)
            {
                tempVector.x = Mathf.Sign(rb.velocity.x) * inAirMoveSpeed;
                tempVector.y = rb.velocity.y;

                rb.velocity = Vector2.Lerp(rb.velocity, tempVector, (moveSpeed - inAirMoveSpeed)/decelSpeed);

                
            }
            if (Mathf.Abs(rb.velocity.x + movementVector.x) <= inAirMoveSpeed || Mathf.Abs(rb.velocity.x + movementVector.x) <= Mathf.Abs(rb.velocity.x))
            {
                rb.velocity += movementVector;
            }
            movementVector.x /= inAirMovementMultiplier;
        }
        


    }
    public void Update()
    {
        // Set animation state machine parameters
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("doubleJumping", doubleJumping);
        anim.SetInteger("Ydir", (int)Mathf.Sign(rb.velocity.y));
        // If the direction of input doesn't match the direction facing, switch the direction facing
        if ((Xdir*t.localScale.x) < 0)
        {
            scaleFlip.x *= t.localScale.x;
            scaleFlip.y *= t.localScale.y;
            scaleFlip.z *= t.localScale.z;
            t.localScale = scaleFlip;
            scaleFlip.Set(-1, 1, 1);
        }
        anim.SetFloat("RunSpeed", Mathf.Abs(movementVector.x));

        // Set the size of the collider to the size of the rendered sprite
        bc2d.size = sr.sprite.bounds.size;
    }

}