using UnityEngine;
using UnityEngine.InputSystem;

// This script is designed to have the OnMove and
// OnJump methods called by a PlayerInput component

public class Movement : MonoBehaviour
{
    Vector2 movementVector;
    Rigidbody2D rigidbody2d;

    [Header("Movement Settings")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpPower = 1f;
    [SerializeField] float inAirMovementSpeed = .5f;

    [Header("Bools")]
    bool isGrounded = false;
    bool isMoving = false;


    // Start is called before the first frame update
    void Start()
    {
        //cache components on start to avoid lag
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        //context.performed is whenever the input changes to a non-zero value
        if (context.performed)
        {
            // read the value for the "move" action each event call
            //*original code*
            //movementVector = new Vector2(context.ReadValue<Vector2>().x, 0f);
            //*after slight optimization to avoid creating vectors every movement*
            movementVector.x = context.ReadValue<Vector2>().x;
            //set isMoving to true
            isMoving = true;
            //change of values located in FixedUpdate() which is called on physics updates (50 per second)
        }
        //context.canceled is whenever the input goes back to default state, called once
        //has to be "else if" because there is also context.started
        else if (context.canceled) 
        {
            //change the isMoving false, but only if the object is on the ground
            if (isGrounded)
                isMoving = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rigidbody2d.AddForce(new Vector2(0f, jumpPower), ForceMode2D.Impulse);
            isGrounded = false;
        }

    }
    void OnCollisionEnter2D(Collision2D other)
    {
        //when it comes in contact with the ground
        //TODO: add conditional to make sure it is the ground, and not the side of something else
        isGrounded = true;
    }

    public void FixedUpdate()
    {
        //this is the same as the code below, Just wanted to use the ternary operator for funsies
        rigidbody2d.velocity = !isMoving ? Vector2.zero : 
            isGrounded ? movementVector * moveSpeed : 
            rigidbody2d.velocity + (movementVector * inAirMovementSpeed);
        /*
        if (!isMoving)
            rigidbody2d.velocity = Vector2.zero;
        else if (isGrounded)
        {
            //if grounded, set movement velocity to
            //the direction of movement multiplied by the default movement speed
            rigidbody2d.velocity = movementVector * moveSpeed;
        }
        //if in the air, change the current velocity by
        //direction multiplied by the default in air movement speed
        else
            rigidbody2d.velocity += movementVector * inAirMovementSpeed;
        */
        
        
    }

}