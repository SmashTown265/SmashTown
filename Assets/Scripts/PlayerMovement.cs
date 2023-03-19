using UnityEngine;
using UnityEngine.InputSystem;

// This script is designed to have the OnMove and
// OnJump methods called by a PlayerInput component

public class PlayerMovement : MonoBehaviour
{
    
    Rigidbody2D rb;
    Vector2 jump = Vector2.up;
    Vector2 tempVector = Vector2.zero;
    [Header("Movement Settings")]
    [SerializeField] float moveSpeed;
    [SerializeField] float jumpPower;
    [SerializeField] float inAirMovementMultiplier;
    [SerializeField] float inAirMoveSpeed;
    [SerializeField] float decelSpeed;

    [Header("Bools")]
    public bool isGrounded = true;
    bool isMoving = false;
    public bool doubleJumping = false;

    [Header("Inputs")]
    public Vector2 movementVector;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 5f;
        jumpPower = 1.5f;
        inAirMovementMultiplier = .15f;
        inAirMoveSpeed = 4.25f;
        decelSpeed = 50f;
        // Cache components on start to avoid lag
        rb = GetComponent<Rigidbody2D>();
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
        doubleJumping = false;
        isGrounded = true;
    }

    public void FixedUpdate()
    {

        // If the player is not giving movement input, and is on the ground
        // change x velocity to 0
        if(!isMoving && isGrounded)
        {
            movementVector.x = 0f;
        }
        if (isGrounded)
        {
            // If grounded, set movement velocity to
            // the direction of movement multiplied by the default movement speed
            movementVector.x *= moveSpeed;
            rb.velocity = movementVector;
            movementVector.x /= moveSpeed;
        }
        // If in the air, and velocity + anticipated movement is less than set movement speed
        // change the current velocity by direction multiplied
        // by the default in air movement speed multiplier

        // TODO: rewrite all this
        else 
        {
            movementVector.x *= inAirMovementMultiplier;
            if (Mathf.Abs(rb.velocity.x) > inAirMoveSpeed)
            {
                tempVector.x = Mathf.Sign(rb.velocity.x) * inAirMoveSpeed;
                tempVector.y = rb.velocity.y;

                rb.velocity = Vector2.Lerp(rb.velocity, tempVector, (moveSpeed - inAirMoveSpeed)/decelSpeed);

                
            }
            if (Mathf.Abs(rb.velocity.x + movementVector.x) <= inAirMoveSpeed)
            {
                rb.velocity += movementVector;
            }
            movementVector.x /= inAirMovementMultiplier;
        }
        


    }

}