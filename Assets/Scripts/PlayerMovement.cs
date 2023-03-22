using JetBrains.Annotations;
using System;
using Unity.VisualScripting;
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
    PlayerInput pI;


    Vector3 scaleFlip = new(-1, 1, 1);
    Vector3 airDodgePos = Vector3.one;
    Vector2 jump = Vector2.up;
    Vector2 tempVector = Vector2.zero;
    Vector3 stickPos = Vector3.zero;
    int Xdir;
    int count;
    int dashDir;
    int dodgeCounter;

    [Header("Character Specific Movement Settings")]
    [Header("Multipliers")]
    [SerializeField] float moveSpeedMultiplier;
    [SerializeField] float jumpPowerMultiplier;
    [SerializeField] float inAirSpeedChangeMultiplier;
    [SerializeField] float dashSpeedMultiplier;
    [SerializeField] float attackSlowMultiplier;
    [SerializeField] float inAirDeceleractionMultiplier;
    [SerializeField] float fastFallMultiplier;
    
    [Header("Constants")]
    [SerializeField] float maxWalkSpeed;
    [SerializeField] float maxInAirMoveSpeed;
    [SerializeField] int dashLength;
    [SerializeField] int dodgeDistance;


    [Header("Bools")]
    public bool isGrounded = false;
    public bool isMoving = false;
    public bool doubleJumping = false;
    public bool isFastFalling = false;
    public bool isDashing_ = false;
    public bool hasDashed = false;
    public bool cancelledWhileDashing = false;
    public bool isHoldingJump = false;
    public bool isAirDodging = false;




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
        pI = GetComponent<PlayerInput>();
        // Initialize all serialized variables so they don't change every time we try something new
        moveSpeedMultiplier = 10f;
        jumpPowerMultiplier = 4.5f;
        inAirSpeedChangeMultiplier = .15f;
        maxInAirMoveSpeed = 6f;
        inAirDeceleractionMultiplier = 75f;
        maxWalkSpeed = .50f;
        dashLength = 15;
        count = 0;
        dashSpeedMultiplier = 1.2f;
        attackSlowMultiplier = 2f;
        fastFallMultiplier = 2f;
        dodgeDistance = 30;
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // Get stickPos for non-deadzone/movement related stuff
        stickPos = context.ReadValue<Vector2>();
        // Read the X value only for the "move" action each event call
        // Set it to 0f if not greater than stick deadzone
        movementVector.x = Mathf.Abs(context.ReadValue<Vector2>().x) > .05f ? context.ReadValue<Vector2>().x : 0f;
        // Fastfall
        if (context.ReadValue<Vector2>().y < -.5f && rb.velocity.y < 0.5f && !isGrounded && !isAirDodging)
            {
                isFastFalling = true;
            }
        // context.performed is whenever the input changes to a non-zero value
        if (context.performed && Mathf.Abs(movementVector.x) > .05f)
        {
            

            if (Mathf.Abs(movementVector.x) > .975f)
            {
                movementVector.x = Mathf.Sign(movementVector.x);
            }
            
            if (Mathf.Abs(movementVector.x) > .1f)
            {
                Xdir = (int)Mathf.Sign(movementVector.x);
            }
            

            // Initial Dash Logic
            if ((Mathf.Abs(movementVector.x) > maxWalkSpeed && isGrounded) && ((movementVector.x * dashDir < 0 && isDashing_) || !hasDashed))
            {
                if (isDashing_)
                {
                    count = 0;
                }
                isDashing_ = true;
                hasDashed = true;
                dashDir = (int)Mathf.Sign(movementVector.x);
            }
            
               
            isMoving = true;
            // Change of values located in FixedUpdate() which is called on physics updates (50 per second)
        }
        // context.canceled is whenever the input goes back to default state, called once
        // has to be "else if" because there is also context.started
        else if (context.canceled) 
        {
            // Update the Phase to cancelled if still dashing
            if (isDashing_)
            {
                cancelledWhileDashing = true;
            }
            hasDashed = false;
            isMoving = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if ((isGrounded || !doubleJumping) && context.performed)
        {
            // Set jump vector y value to jumpPowerMultiplier
            isHoldingJump = true;
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
            cancelledWhileDashing = false;
            isDashing_ = false;
            hasDashed = false;
        }
        if (context.canceled)
        {
            isHoldingJump = false;
        }

    }
    public void OnAirDodge(InputAction.CallbackContext context)
    {
        if (context.performed && !isAirDodging)
        {
            airDodgePos = stickPos * 2;
            rb.gravityScale = 0f;
            isAirDodging = true;
        }
    }
    public void OnFastFall(InputAction.CallbackContext context)
    {
        if(context.performed && rb.velocity.y < 3f && !isGrounded)
        {
            isFastFalling = true;
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
        // **Dash Movement**

        // Is the dash done yet?
        if (count >= dashLength)
        {
            count = 0;
            isDashing_ = false;
        }
        if (!isDashing_ && cancelledWhileDashing)
        {
            hasDashed = false;
            cancelledWhileDashing = false;
        }
        // If dashing and on the ground
        if (isDashing_ && count < dashLength && isGrounded)
        {
            // Update the velocity to the direction the player is dashing, at the normal moveSpeed with a multiplication modifier of the dash speed
            rb.velocity = dashDir * moveSpeedMultiplier * dashSpeedMultiplier * Vector2.right;
            count++;
        }

        // **Regular Ground Movement**

        // Stick input should only affect velocity in the X direction
        // therefore all velocity changes with respect to magnitude of movement input should leave Y value alone
        else if (isGrounded && !isDashing_ && isMoving && !pa.isAttacking)
        {
            // If grounded and not dashing,
            // interpolate the velocity to the movementVector multiplied by
            // the movement speed over ten fixed updates

            //rb.velocity.Set(Mathf.Lerp(rb.velocity.x, movementVector.x * moveSpeedMultiplier, 10), rb.velocity.y);
            movementVector.x *= moveSpeedMultiplier;
            rb.velocity = movementVector;
            movementVector.x /= moveSpeedMultiplier;

            if (!isMoving)
            {
                //rb.velocity = Vector2.zero;
            }
            else if (pa.isAttacking)
            {
                //rb.velocity /= attackSlowMultiplier;
            }

        }
        
        // **In Air Movement**

        else 
        {
            if (isAirDodging)
            {
                t.position = Vector3.Lerp(t.position, (t.position + airDodgePos), dodgeDistance);
            }
            if(isHoldingJump && rb.velocity.y > 0f)
            {
               // rb.AddForce(new Vector2(0f, 5f), ForceMode2D.Force);
            }

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
            
        }
        // If not fast falling, reset gravity scale
        if (isFastFalling && rb.velocity.y <= 0 && rb.velocity.y > -10f)
        {

            rb.AddForce(new Vector2(0f,-3f), ForceMode2D.Impulse);
            //rb.velocity.Set(rb.velocity.x, -100f);
            print("Fast Fall");
            //rb.gravityScale = defaultGravityScale * fastFallMultiplier;
        }
        else
        {
            //rb.gravityScale = defaultGravityScale;
        }
        
        
        

    }
    public void Update()
    {
        // Set animation state machine parameters
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isMoving", isMoving || isDashing_);
        anim.SetBool("doubleJumping", doubleJumping);
        anim.SetInteger("Ydir", (int)Mathf.Sign(rb.velocity.y));
        anim.SetFloat("RunSpeed", Mathf.Abs(rb.velocity.x) / moveSpeedMultiplier);

        // Set sprite direction
        if (((Xdir * t.localScale.x) < 0  && !isDashing_ ) || (isDashing_ && (t.localScale.x * dashDir) < 0))
        {
            // If the direction of input doesn't match the direction facing, switch the direction facing
            // Unless dashing
            scaleFlip.x *= t.localScale.x;
            scaleFlip.y *= t.localScale.y;
            scaleFlip.z *= t.localScale.z;
            t.localScale = scaleFlip;
            scaleFlip.Set(-1, 1, 1);
        }
        // Set the size of the collider to the size of the rendered sprite
        // ! TODO: make a better collider system, the swords and capes shouldn't be considered in hitboxes
        bc2d.size = sr.sprite.bounds.size;
    }

}