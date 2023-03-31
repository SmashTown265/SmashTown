using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


// This Script has the OnMove and OnJump methods, as well as all animation parameters associated with movement

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private Transform t;
    private BoxCollider2D bc2d;
    private SpriteRenderer sr;
    private PlayerAttack pa;

    private Vector3 airDodgePos = Vector3.one;
    private Vector2 jump = Vector2.up;
    private Vector2 tempVector = Vector2.zero;
    private State prevState = State.None;

    private float gravityScale;
    [HideInInspector] public int Xdir = 1;
    private int dashDir;

    [Header("Character Specific Movement Settings")] 
    [Header("Multipliers")] 
    [SerializeField] private float jumpPowerMultiplier;
    [SerializeField] private float inAirSpeedChangeMultiplier;
    [SerializeField] private float dashSpeedMultiplier;
    [SerializeField] private float inAirDeceleractionMultiplier;
    [SerializeField] private float airDodgeSpeed = 20f;

    [Header("Constants")] [SerializeField] private float maxWalkSpeed;
    [SerializeField] private float maxInAirMoveSpeed;
    [SerializeField] private int dashLength;
    [SerializeField] private int dodgeDistance;
    [SerializeField] private float maxMoveSpeed;

    [Header("Counters")] 
    public int count;
    public int dodgeCounter;

    [Header("Current Input")] 
    public Vector2 stickPos = Vector2.zero;

    [Header("Input used for player movement")]
    public Vector2 movementVector = Vector2.zero;

    [Header("States")] [Header("current state")]
    public State playerState = State.Idle;

    [Header("all states")] 
    public bool ground;
    public bool idle;
    public bool running;
    public bool dashing;
    public bool inAir;
    public bool jumping;
    public bool doubleJumping;
    public bool airDodging;
    public bool fastFalling;
    public bool attacking;

    // Start is called before the first frame update
    private void Start()
    {
        // Cache components on start to avoid lag
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        t = GetComponent<Transform>();
        bc2d = GetComponent<BoxCollider2D>();
        sr = GetComponent<SpriteRenderer>();
        pa = GetComponent<PlayerAttack>();
        GameObject player2 = GameObject.FindWithTag("Player 2");
        BoxCollider2D p2Collider = player2.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(bc2d, p2Collider);

        // Initialize all serialized variables so they don't change every time we try something new
        maxMoveSpeed = 10f;
        jumpPowerMultiplier = 4.5f;
        inAirSpeedChangeMultiplier = .15f;
        maxInAirMoveSpeed = 6f;
        inAirDeceleractionMultiplier = 75f;
        maxWalkSpeed = .70f;
        dashLength = 2;
        count = 0;
        dashSpeedMultiplier = 1.2f;
        dodgeDistance = 15;
        count = 0;
        dodgeCounter = 0;
        gravityScale = rb.gravityScale;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
	    // Get stickPos for non-deadzoned/movement related stuff
	    stickPos = context.ReadValue<Vector2>();

        // context.performed is whenever the input changes to a non-zero value
        if (context.performed)
        {
            // Read the X value only for the "move" action each event call
            // Set it to 0f if not greater than stick deadzone
            movementVector.x = Mathf.Abs(context.ReadValue<Vector2>().x) > .05f ? context.ReadValue<Vector2>().x : 0f;

            // Fastfall if stick is more than halfway actuated down, and vertical velocity is low or downward
            if (context.ReadValue<Vector2>().y < -.5f && rb.velocity.y < 1.5f &&
                playerState.HasFlags(State.InAir) && playerState != State.AirDodging)
            {
                playerState.AddFlag(State.FastFalling);
            }

            if (Mathf.Abs(movementVector.x) > .975f)
            {
                movementVector.x = Mathf.Sign(movementVector.x);
            }

            if (Mathf.Abs(movementVector.x) > .1f)
            {
                Xdir = (int)Mathf.Sign(movementVector.x);
            }

            // Initial Dash Logic
            if (playerState.CheckFlags(State.Ground, State.Dashing) &&
                Mathf.Abs(movementVector.x) > maxWalkSpeed)
            {
                playerState = State.Dashing;
                dashDir = (int)Mathf.Sign(movementVector.x);
            }

            // Change of values located in FixedUpdate() which is called on physics updates (50 per second)
        }
        // context.canceled is whenever the input goes back to default state, called once
        // has to be "else if" because there is also context.started
        else if (context.canceled)
        {
            movementVector.x = 0f;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!playerState.HasEachFlag(State.Jumping | State.DoubleJumping) && !playerState.HasFlag(State.Attacking) &&
            context.performed)
        {
            StartCoroutine(JumpRoutine());
        }

        if (context.canceled)
        {
            // short hop?
        }
    }

    public void OnAirDodge(InputAction.CallbackContext context)
    {
        if (context.performed && !playerState.HasFlags(State.Ground) && !playerState.HasFlags(State.AirDodging))
        {
            dodgeCounter = 0;
            airDodgePos = (Vector3)stickPos.normalized * airDodgeSpeed;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            // ! TODO add co-routine for gravity scale
            prevState = playerState;
            playerState = State.AirDodging;
            //rb.AddForce(airDodgePos, ForceMode2D.Impulse);
        }
    }

    public void OnFastFall(InputAction.CallbackContext context)
    {
        // If moving at a slow speed upward, or downward at all, allow for toggle of fastFalling
        if (context.performed && rb.velocity.y < 3f && playerState.HasFlags(State.InAir))
        {
            playerState.AddFlag(State.FastFalling);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // When it comes in contact with the ground

        if (other.gameObject.tag is "Ground" or "Platform" && playerState != State.AirDodging)
        {
            playerState = playerState.HasFlags(State.Attacking) ? State.Attacking | (rb.velocity.x != 0 ? State.Running : State.Idle) : (rb.velocity.x != 0 ? State.Running : State.Idle);
            rb.gravityScale = gravityScale;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        // When it Stays in contact with the ground

        if (other.gameObject.tag is "Ground" or "Platform" && playerState != State.Jumping && playerState != State.AirDodging && !playerState.HasFlags(State.Ground))
        {
	        playerState = playerState.HasFlags(State.Attacking) ? State.Attacking | (rb.velocity.x != 0 ? State.Running : State.Idle) : (rb.velocity.x != 0 ? State.Running : State.Idle);
            rb.gravityScale = gravityScale;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // When it Leaves contact with the ground

        if (other.gameObject.tag is "Ground" or "Platform")
        {
            print($"{playerState.HasFlags(State.Ground)} This should be false - player is in the air");
        }
    }
    // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public void OnTeleport(InputAction.CallbackContext context)
    {
        if (context.performed)
            t.position = new Vector3(0, 0, 0);
    }

    public void FixedUpdate()
    {
        // State updates
        if (playerState.HasFlags(State.Ground) && playerState != State.Dashing && !playerState.HasFlags(State.Attacking) && pa.attackTimer == 0)
        {
            
            if (rb.velocity.x == 0f)
            {
                playerState.AddFlag(State.Idle);
                playerState.RemoveFlag(State.Running | State.Dashing);
            }
            else
            {
                playerState.AddFlag(State.Running);
                playerState.RemoveFlag(State.Idle);
            }
        }
        // Sort of State machine
	    switch (playerState)
        {
            case State.Idle:
                anim.SetInteger("playerState", (int)playerState);
                break;
            case State.Running or State.Running | State.Dashing:
                anim.SetInteger("playerState", (int)State.Running);
                // ** Regular Ground Movement **

                // If grounded and not dashing or attacking
                // interpolate the velocity to the movementVector multiplied by
                // the movement speed over ten fixed updates
                if (Mathf.Abs(rb.velocity.x) < maxMoveSpeed && movementVector.x != 0)
                {
                    rb.velocity = Vector2.Lerp(rb.velocity, movementVector * maxMoveSpeed, 4);
                }
                // Old movement Code - works well, revert to it if current is buggy
                /*movementVector.x *= maxMoveSpeed;
                rb.velocity = movementVector;
                movementVector.x /= maxMoveSpeed;*/

                break;
            case State.Dashing:
                anim.SetInteger("playerState", (int)playerState);

                // **Dash Movement**

                if (count >= dashLength)
                {
                    count = 0;
                    playerState.AddFlag(State.Running);
                }
                else
                {
                    // Update the velocity to the direction the player is dashing, at the normal moveSpeed with a multiplication modifier of the dash speed
                    rb.velocity = dashDir * maxMoveSpeed * dashSpeedMultiplier * Vector2.right;
                    count++;
                }
                break;
            case State.AirDodging:
                anim.SetInteger("playerState", (int)State.Jumping);
                if (dodgeCounter < dodgeDistance)
                {
	                rb.velocity = Vector2.zero;
	                rb.AddForce(airDodgePos, ForceMode2D.Impulse);
                    dodgeCounter += 1;
                }
                else if (dodgeCounter == dodgeDistance)
                {
                    dodgeCounter = 0;
                    rb.velocity = Vector2.zero;
                    playerState.AddFlag(prevState);
                    rb.gravityScale = gravityScale;
                }
                break;
            case var x when x.HasFlags(State.InAir): //or State.Jumping or State.Jumping | State.DoubleJumping:
                anim.SetInteger("playerState", (int)State.Jumping);
                // **In Air Movement**

                // If in the air, and velocity + anticipated movement is less than set movement speed
                // change the current velocity by direction multiplied
                // by the default in air movement speed multiplier

                movementVector *= inAirSpeedChangeMultiplier;
                if (Mathf.Abs(rb.velocity.x) > maxInAirMoveSpeed)
                {
                    tempVector.x = Mathf.Sign(rb.velocity.x) * maxInAirMoveSpeed;
                    tempVector.y = rb.velocity.y;

                    rb.velocity = Vector2.Lerp(rb.velocity, tempVector,
                        (maxMoveSpeed - maxInAirMoveSpeed) / inAirDeceleractionMultiplier);
                }

                if (Mathf.Abs(rb.velocity.x + movementVector.x) <= maxInAirMoveSpeed ||
                    Mathf.Abs(rb.velocity.x + movementVector.x) <= Mathf.Abs(rb.velocity.x))
                {
                    rb.velocity += movementVector;
                }

                movementVector /= inAirSpeedChangeMultiplier;
                if (playerState.HasFlags(State.FastFalling) && rb.velocity.y is <= 0 and > -10f)
                {
	                rb.AddForce(Vector2.down * 3f, ForceMode2D.Impulse);
                }
                break;
            case var x when x.HasFlags(State.Attacking):
                anim.SetInteger("playerState", (int)playerState);
                break;
        }
	    
	    
	   
        ground = playerState.HasFlags(State.Ground);
        idle = playerState.HasFlags(State.Idle);
        running = playerState.HasFlags(State.Running);
        dashing = playerState.HasFlags(State.Dashing);
        inAir = playerState.HasFlags(State.InAir);
        jumping = playerState.HasFlags(State.Jumping);
        doubleJumping = playerState.HasFlags(State.DoubleJumping);
        airDodging = playerState.HasFlags(State.AirDodging);
        fastFalling = playerState.HasFlags(State.FastFalling);
        attacking = playerState.HasFlags(State.Attacking);
    }
    public void Update()
    {
        // Set animation state machine parameters
        anim.SetBool("isGrounded", playerState.HasFlag(State.Ground));
        anim.SetBool("isMoving", !playerState.HasFlag(State.Idle | State.Attacking));
        anim.SetBool("doubleJumping", playerState.HasFlag(State.DoubleJumping)); // ? what does this get used for?
        anim.SetInteger("Ydir", (int)rb.velocity.y);
        anim.SetFloat("RunSpeed", Mathf.Abs(rb.velocity.x) / maxMoveSpeed);

        //anim.SetInteger("playerState", (int)playerState);
        // Set sprite direction
        if ((Xdir * t.localScale.x) < 0  && !playerState.HasFlags(State.InAir) || playerState == State.Dashing && (t.localScale.x * dashDir) < 0)
        {
            // If the direction of input doesn't match the direction facing, switch the direction facing
            // Unless dashing
            t.localScale = FlipX(t.localScale);
        }
        // Set the size of the collider to the size of the rendered sprite
        // ! TODO: make a better collider system, the swords and capes shouldn't be considered in hit boxes - even if they will be
        //if(!playerState.HasFlag(State.Attacking))
			//bc2d.size = sr.sprite.bounds.size;
    }

    // Jump coroutine
    public IEnumerator JumpRoutine()
    {
        jump.y = jumpPowerMultiplier;

        // Set jump state
        if (playerState.HasFlags(State.Jumping))
        {
            playerState.AddFlag(State.DoubleJumping);
        }
        else
        {
            playerState = State.Jumping;
        }

        // If moving in opposite direction than stick input during jump input, stop moving to allow for change in direction
        tempVector.x = rb.velocity.x * movementVector.x < 0 ? 0f : rb.velocity.x;

        // Set the y velocity to 0 to allow for double jump
        tempVector.y = 0f;
        rb.velocity = tempVector;

        // If not doubleJumping, wait for jumpSquat
        if (!playerState.HasFlags(State.DoubleJumping))
        {
            yield return new WaitForSeconds(2 / 24f);
        }

        // Jump Force
        rb.AddForce(jump, ForceMode2D.Impulse);
    }
    private static Vector3 FlipX(Vector3 toFlip)
    {
        toFlip.x *= -1;
        return toFlip;
    }
}
// States enum
[Flags]
public enum State
{
    None = 0,
    Ground = 1,
    Idle = Ground + 2,
    Running = Ground + 4,
    Dashing = Ground + 8,
    InAir = 16,
    Jumping = InAir + 32,
    DoubleJumping = InAir + 64,
    AirDodging = InAir + 128,
    FastFalling = InAir + 256,
    Attacking = 512,
}
public static class Extensions
{
    public static bool HasFlags(this State lhs, State rhs)
    {
        if (rhs is State.Ground or State.InAir)
            return (lhs & rhs) == rhs;
        else
            return (lhs & (rhs & ~(State.Ground | State.InAir))) != 0;
    }

    public static bool CheckFlags(this State lhs, State hasAnyOfTheseStates, State doesNotHaveAnyOfTheseStates) =>
	    lhs.HasFlags(hasAnyOfTheseStates) && !lhs.HasFlags(doesNotHaveAnyOfTheseStates);

    public static bool CheckEachFlag(this State lhs, State hasAllOfTheseStates, State doesNotHaveAnyOfTheseStates) =>
	    lhs.HasEachFlag(hasAllOfTheseStates) && !lhs.HasFlags(doesNotHaveAnyOfTheseStates);
    public static bool HasEachFlag(this State lhs, State rhs) => (lhs & rhs) == rhs;
    public static void AddFlag(this ref State lhs, State rhs) => lhs |= rhs;
    public static void RemoveFlag(this ref State lhs, State rhs)
    { 
        lhs &= ~(rhs & ~(State.Ground | State.InAir));
        if (rhs == State.Attacking)
        {
            return;
        }
        if ((int)lhs < (int)State.InAir * 2)
        {
            lhs &= ~State.InAir;
        }
        else if ((int)lhs > (int)State.InAir * 2)
        {
            lhs &= ~State.Ground;
        }
    }
}