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
    private State someState = State.AirDodging | State.Running;

    private float gravityScale;
    private int Xdir;
    private int dashDir;

    [Header("Character Specific Movement Settings")] 
    [Header("Multipliers")] 
    [SerializeField] private float jumpPowerMultiplier;
    [SerializeField] private float inAirSpeedChangeMultiplier;
    [SerializeField] private float dashSpeedMultiplier;
    [SerializeField] private float inAirDeceleractionMultiplier;
    [SerializeField] private float airDodgeDistance = 2f;

    [Header("Constants")] [SerializeField] private float maxWalkSpeed;
    [SerializeField] private float maxInAirMoveSpeed;
    [SerializeField] private int dashLength;
    [SerializeField] private int dodgeSpeed;
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

        // Initialize all serialized variables so they don't change every time we try something new
        maxMoveSpeed = 10f;
        jumpPowerMultiplier = 4.5f;
        inAirSpeedChangeMultiplier = .15f;
        maxInAirMoveSpeed = 6f;
        inAirDeceleractionMultiplier = 75f;
        maxWalkSpeed = .50f;
        dashLength = 5;
        count = 0;
        dashSpeedMultiplier = 1.2f;
        dodgeSpeed = 30;
        count = 0;
        dodgeCounter = 0;
        gravityScale = rb.gravityScale;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // context.performed is whenever the input changes to a non-zero value
        if (context.performed)
        {
            // Get stickPos for non-deadzoned/movement related stuff
            stickPos = context.ReadValue<Vector2>();

            // Read the X value only for the "move" action each event call
            // Set it to 0f if not greater than stick deadzone
            movementVector.x = Mathf.Abs(context.ReadValue<Vector2>().x) > .05f ? context.ReadValue<Vector2>().x : 0f;

            // Fastfall if stick is more than halfway actuated down, and vertical velocity is low or downward
            if (context.ReadValue<Vector2>().y < -.5f && rb.velocity.y < 1.5f &&
                playerState.HasEachFlag(State.InAir | State.Jumping))
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
            if (!playerState.HasFlags(State.Dashing) && playerState.HasFlags(State.Ground) &&
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
        if (context.performed && !playerState.HasFlags(State.Ground | State.AirDodging))
        {
            dodgeCounter = 0;
            airDodgePos = (Vector3)stickPos * airDodgeDistance;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            prevState = playerState;
            playerState = State.AirDodging;
            rb.AddForce(airDodgePos, ForceMode2D.Impulse);
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
        // TODO: add conditional to make sure it is the ground, and not the side of something else
        if (other.gameObject.tag is "Ground" or "Platform" && !playerState.HasFlags(State.AirDodging))
        {
            playerState = playerState.HasFlags(State.Attacking) ? State.Attacking | (rb.velocity.x != 0 ? State.Running : State.Idle) : (rb.velocity.x != 0 ? State.Running : State.Idle);
            rb.gravityScale = gravityScale;
        }
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        // When it comes in contact with the ground
        // TODO: add conditional to make sure it is the ground, and not the side of something else
        if (other.gameObject.tag is "Ground" or "Platform" && playerState != State.Jumping && !playerState.HasFlags(State.AirDodging))
        {
            playerState.AddFlag(State.Ground);
            rb.gravityScale = gravityScale;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        // When it comes in contact with the ground
        // TODO: add conditional to make sure it is the ground, and not the side of something else
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
        if (playerState.HasFlags(State.Ground) && playerState != State.Dashing)
        {
            //&& !playerState.HasFlags(State.attacking)
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
        // **Dash Movement**

        // Is the dash done yet?
        // ? Should this be a coroutine?
        if (count >= dashLength)
        {
            count = 0;
            playerState.AddFlag(State.Running);
        }

        // If dashing 
        if (playerState == State.Dashing && count < dashLength)
        {
            // Update the velocity to the direction the player is dashing, at the normal moveSpeed with a multiplication modifier of the dash speed
            rb.velocity = dashDir * maxMoveSpeed * dashSpeedMultiplier * Vector2.right;
            count++;
        }

        // **Regular Ground Movement**

        // Stick input should only affect velocity in the X direction
        // therefore all velocity changes with respect to magnitude of movement input should leave Y value alone
        else if (playerState.HasFlags(State.Running) && playerState != State.Dashing &&
                 !playerState.HasFlags(State.Attacking))
        {
            // If grounded and not dashing or attacking
            // interpolate the velocity to the movementVector multiplied by
            // the movement speed over ten fixed updates
            if (Mathf.Abs(rb.velocity.x) < maxMoveSpeed && movementVector.x != 0)
            {
                rb.velocity = Vector2.Lerp(rb.velocity, movementVector * maxMoveSpeed, 4);
            }
            /*movementVector.x *= maxMoveSpeed;
            rb.velocity = movementVector;
            movementVector.x /= maxMoveSpeed;*/
        }

        // **In Air Movement**

        else if (playerState.HasFlags(State.InAir) && playerState != State.AirDodging)
        {
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
            // If input is triggered for fast fall, and the player is moving down, double gravity scale
        }

        // If fast falling, and not already falling faster than -10f, add downward impulse force
        if (playerState.HasFlags(State.FastFalling) && rb.velocity.y is <= 0 and > -10f)
        {
            rb.AddForce(Vector2.down * 3f, ForceMode2D.Impulse);
        }

        // If airDodging
        if (playerState == State.AirDodging)
        {
            if (dodgeCounter < dodgeSpeed)
            {
                // Go to airDodgePos from current position over the course of dodgeSpeed fixedUpdate loops
                // Position is set to the difference between the two positions multiplied by the percentage of airDodge completed
                //t.position = Vector3.Lerp(t.position, airDodgePos, dodgeCounter / (float)dodgeSpeed);
                dodgeCounter += 1;
            }
            else if (dodgeCounter == dodgeSpeed)
            {
                dodgeCounter = 0;
                playerState.AddFlag(prevState);
                rb.gravityScale = gravityScale;
            }
        }

        print((int)someState);
        someState.RemoveFlag(State.AirDodging);
        //someState &= ~State.airDodging;
        print((int)someState);
        someState.AddFlag(State.AirDodging);
        //someState |= State.airDodging;
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
        switch(playerState)
        {
            case State.Idle:
                anim.SetInteger("playerState", (int)playerState);
                break;
            case State.Running or State.Running | State.Dashing:
                anim.SetInteger("playerState", (int)State.Running);
                break;
            case State.Dashing:
                anim.SetInteger("playerState", (int)playerState);
                break;
            case State.Jumping or State.Jumping | State.DoubleJumping:
                anim.SetInteger("playerState", (int)State.Jumping);
                break;
            case State.DoubleJumping:
                anim.SetInteger("playerState", (int)playerState);
                break;
            case State.AirDodging:
                anim.SetInteger("playerState", (int)playerState);
                break;
            case State.FastFalling:
                anim.SetInteger("playerState", (int)playerState);
                break;
            case var x when x.HasFlags(State.Attacking):
                anim.SetInteger("playerState", (int)playerState);
                break;
        }
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