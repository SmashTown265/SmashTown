using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


// This Script has the OnMove and OnJump methods, as well as all animation parameters associated with movement

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
    float defaultGravityScale;
    int Xdir;
    int count;
    int dashDir;
    

    [Header("Character Specific Movement Settings")]
    [Header("Multipliers")]
    [SerializeField] float moveSpeedMultiplier;
    [SerializeField] float jumpPowerMultiplier;
    [SerializeField] float inAirSpeedChangeMultiplier;
    [SerializeField] float dashSpeedMultiplier;
    [SerializeField] float attackSlowMultiplier;
    [SerializeField] float inAirDeceleractionMultiplier;
    [SerializeField] float fastFallMultiplier;
    [SerializeField] int dashLengthMultiplier;
    [Header("Constants")]
    [SerializeField] float maxWalkSpeed;
    [SerializeField] float maxInAirMoveSpeed;
    


    [Header("Bools")]
    public bool isGrounded = false;
    public bool isMoving = false;
    public bool doubleJumping = false;
    public bool isFastFalling = false;
    public bool isDashing_ = false;
    public bool hasDashed = false;




    [Header("Current Input after processing")]
    public Vector2 movementVector;

    // Keeps other scripts from changing it
    public bool isDashing => isDashing_;

    // Start is called before the first frame update
    void Start()
    {
        // Cache components on start to avoid lag
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        t = GetComponent<Transform>();
        bc2d = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        pa = GetComponent<PlayerAttack>();
        // Initialize all serialized variables so they don't change every time we try something new
        moveSpeedMultiplier = 10f;
        jumpPowerMultiplier = 4.5f;
        inAirSpeedChangeMultiplier = .15f;
        maxInAirMoveSpeed = 6f;
        inAirDeceleractionMultiplier = 75f;
        maxWalkSpeed = .50f;
        dashLengthMultiplier = 25;
        count = 0;
        dashSpeedMultiplier = 1.2f;
        attackSlowMultiplier = 2f;
        fastFallMultiplier = 2f;
        defaultGravityScale = rb.gravityScale;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        
        // context.performed is whenever the input changes to a non-zero value
        if (context.performed && !isDashing_)
        {
            // Read the X value only for the "move" action each event call
            movementVector.x = context.ReadValue<Vector2>().x;

            if (Mathf.Abs(movementVector.x) > .975f)
            {
                movementVector.x = Mathf.Sign(movementVector.x);
            }
            if (Mathf.Abs(movementVector.x) > .1f)
            {
                Xdir = (int)Mathf.Sign(movementVector.x);
            }
            if (context.ReadValue<Vector2>().y < 0)
            {
                isFastFalling = true;
            }

            // Initial Dash Logic
            if (Mathf.Abs(movementVector.x) > maxWalkSpeed && !hasDashed)
            {
                isDashing_ = true;
                dashDir = (int)Mathf.Sign(movementVector.x);
            }
            
               
            isMoving = true;
            // Change of values located in FixedUpdate() which is called on physics updates (50 per second)
        }
        // context.canceled is whenever the input goes back to default state, called once
        // has to be "else if" because there is also context.started
        else if (context.canceled) 
        {
            print("Canceled");
            // Set isMoving to false
            // Set hasDashed to false
            // If input is both cancelled and not still dashing
            if (!isDashing_)
            {
                hasDashed = false;
                //isMoving = false;
            }
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if ((isGrounded || !doubleJumping) && context.performed)
        {
            // 
            jump.y = jumpPowerMultiplier;
            if (rb.velocity.x * movementVector.x < 0)
            {
                tempVector.x = 0f;
            }
            else
            {
                tempVector.x = rb.velocity.x;
            }
            tempVector.y = 0f;
            rb.velocity = tempVector;
            rb.AddForce(jump, ForceMode2D.Impulse);
            doubleJumping = !isGrounded;
            isGrounded = false;
            isFastFalling = false;
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
            isFastFalling = false;
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

        // Is there movement in the X direction?
        isMoving = rb.velocity.x == 0;
        

        // **Dash Movement**

        // Is the dash done yet?
        if (count >= dashLengthMultiplier)
        {
            count = 0;
            isDashing_ = false;
        }
        // If the dash isn't, and player is on the ground
        if (isDashing_ && count < dashLengthMultiplier && isGrounded)
        {
            // update the movementVector X value to be the same as the direction being dashed in, for other scripts logic
            movementVector.x = ((movementVector.x * dashDir) <= 0) ? movementVector.x * dashDir : movementVector.x;
            // Update the velocity to the direction the player is dashing, at the normal moveSpeed with a multiplication modifier of the dash speed
            rb.velocity = dashDir * moveSpeedMultiplier * dashSpeedMultiplier * Vector2.right;
            count++;
        }

        // **Regular Ground Movement**

        // Stick input should only affect velocity in the X direction
        // therefore all velocity changes with respect to magnitude of movement input should leave Y value alone
        else if (isGrounded && !isDashing_)
        {
            // If grounded and not dashing,
            // interpolate the velocity to the movementVector multiplied by
            // the movement speed over ten fixed updates

            rb.velocity.Set(Mathf.Lerp(rb.velocity.x, movementVector.x * moveSpeedMultiplier, 10), rb.velocity.y);
            movementVector.x *= moveSpeedMultiplier;
            rb.velocity = Vector2.Lerp(rb.velocity, movementVector, 10);
            movementVector.x /= moveSpeedMultiplier;

            if (!isMoving)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 10);
            }
            else if (pa.isAttacking)
            {
                rb.velocity /= attackSlowMultiplier;
            }

        }
        
        // **In Air Movement**

        else 
        {
            // If in the air, and velocity + anticipated movement is less than set movement speed
            // change the current velocity by direction multiplied
            // by the default in air movement speed multiplier

            //Input should only affect the X direction velocity
            movementVector.x *= inAirSpeedChangeMultiplier;
            if (Mathf.Abs(rb.velocity.x) > maxInAirMoveSpeed)
            {
                tempVector.x = Mathf.Sign(rb.velocity.x) * maxInAirMoveSpeed;
                tempVector.y = rb.velocity.y;

                rb.velocity = Vector2.Lerp(rb.velocity, tempVector, (moveSpeedMultiplier - maxInAirMoveSpeed)/inAirDeceleractionMultiplier);
            }
            if (Mathf.Abs(rb.velocity.x + movementVector.x) <= maxInAirMoveSpeed || Mathf.Abs(rb.velocity.x + movementVector.x) <= Mathf.Abs(rb.velocity.x))
            {
                rb.velocity += movementVector;
            }
            movementVector.x /= inAirSpeedChangeMultiplier;
            // If input is triggered for fast fall, and the player is moving down, double gravity scale
            if (isFastFalling && rb.velocity.y <= 0)
            {
                rb.gravityScale = defaultGravityScale * fastFallMultiplier;
            }
        }
        // If not fast falling, reset gravity scale
        if (!isFastFalling)
        {
            rb.gravityScale = defaultGravityScale / fastFallMultiplier;
        }

    }
    public void Update()
    {
        // Set animation state machine parameters
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("doubleJumping", doubleJumping);
        anim.SetInteger("Ydir", (int)Mathf.Sign(rb.velocity.y));
        anim.SetFloat("RunSpeed", Mathf.Abs(movementVector.x));

        // Set sprite direction
        if ((Xdir * t.localScale.x) < 0 || (isDashing_ && (dashDir * t.localScale.x) < 0))
        {
            // If the direction of input doesn't match the direction facing, switch the direction facing
            // Unless dashing
            t.localScale.Scale(scaleFlip);
        }
        // Set the size of the collider to the size of the rendered sprite
        // ! TODO: make a better collider system, the swords and capes shouldn't be considered in hitboxes
        bc2d.size = sr.sprite.bounds.size;
    }

}