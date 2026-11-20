using UnityEngine;
using UnityEngine.InputSystem;

            /////////////// INFORMATION ///////////////
// This script is intended to work on a GameObject with
// a Rigidbody2D-component (tip: it probably needs constraints),
// a CapsuleCollider2D-component (or other Collider2D-shape that fits your character)
// and a PlayerInput-component.

// Additionally this script also needs to reference a Collider2D to use for ground-check,
// This groundcheck-collider does not need to be placed on the same object, it could instead be placed on a child object.
// This groundcheck-collider should be a trigger-collider.

// Use UnityEvents in the PlayerInput-component to call movement-functions in this script.


[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D),typeof(Animator))]

public class CustomizeablePlayerMovement: MonoBehaviour
{
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField]private float currentSpeed;
    [SerializeField] private float jumpForce = 10f;
    
    //SOUND
    private AudioSource audioSource;
    
    // Physics and collision
    [SerializeField] private Rigidbody2D rigidbody;
    [SerializeField] private CapsuleCollider2D groundCheckCollider;
    [SerializeField] private LayerMask groundLayer = ~0; // ~0 is referring to EVERY layer.
    
    // Handle input
    public bool controlEnabled { get; set; } = true; // You can edit this variable from Unity Events
    private Vector2 moveInput;
    private Vector2 velocity;
    private bool jumpInput;


    [SerializeField] private bool jumpReleased;
    [SerializeField] private bool wasGrounded;
    [SerializeField] private bool isGrounded;
    
    
    //-----------------------------------------------------------
    
    
    //Graphicx
    [Header("Graphics")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody.gravityScale = 0; // Set gravity scale to 0 so player won't "fall". We will apply our own gravity!
        groundCheckCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();


        currentSpeed = walkSpeed;

    }
    
    void Update()
    {
        //---Handle vertical velocity---
        isGrounded = IsGrounded();
        ApplyGravity();
        
        // Apply jump-input:
        if (jumpInput && wasGrounded)
        {
            velocity.y = jumpForce;
            jumpInput = false;
            
        }
        
        // Check if character lost contact with ground this frame
        if (wasGrounded == true && isGrounded == false)
        {
            if (velocity.y < 0)
            {
                // Has fallen. Play fall sound and/or trigger fall animation etc
                
                
            }
            else
            {
                animator.SetBool("isGrounded",false);
                // Has jumped. Play jump sound and/or trigger jump animation etc
                audioSource.Play();

            }
        }
        // Check if character gained contact with ground this frame
        else if (wasGrounded == false && isGrounded == true)
        {
            jumpReleased = false;
            // Has landed, play landing sound and trigger landing animation
            
            

        }
        
        rigidbody.linearVelocity = velocity;
        wasGrounded = isGrounded;
        
        //---Handle horizontal velocity---
        velocity.x = moveInput.x * currentSpeed;
        
        // Write movement animation code here. (Suggestion: send your current velocity into the Animator for both the x- and y-axis.)
        // animator.SetFloat("HorizontalMovement", moveInput.x)
        
        animator.SetFloat("xVelocity", Mathf.Abs(velocity.x));
        animator.SetFloat("yVelocity",velocity.y);
        
        if(isGrounded)animator.SetBool("isGrounded",true);
        

        
        
        
        
        
        // Flip sprite according to direction (if a sprite renderer has been assigned)
        if (spriteRenderer)
        {
            if (velocity.x < -0.01f)
                spriteRenderer.flipX = false;
            else if (velocity.x > 0.01f)
                spriteRenderer.flipX = true;
        }
    }

    private bool IsGrounded()
    {
        // Is our groundCheckCollider touching the groundLayer? If so, return the value "true"
        if (groundCheckCollider.IsTouchingLayers(groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void ApplyGravity()
    {
        // Applies a set gravity for when player is grounded
        if (isGrounded && velocity.y < 0.0f)
        {
            velocity.y = -1.0f;
        }
        // Updates fall speed with gravity if object isn't grounded
        else
        {
            // Is Jumping upwards
            if (velocity.y > 0)
            {
                float deceleration = 1.3f;
                if (jumpReleased) // shorter jump height when releasing the jump-key
                {
                    deceleration = 5;
                }
                // you can add a gravity multiplier here... but how?
                velocity.y += Physics2D.gravity.y * deceleration * Time.deltaTime;
            }
            // Is Falling
            else
            {
                // you can add a gravity multiplier here... but how?
                velocity.y += Physics2D.gravity.y * Time.deltaTime;
            }
        }
    }

    // Handle Move-input
    // This method can be triggered through the UnityEvent in PlayerInput
    public void OnMove(InputAction.CallbackContext context)
    {
        if (controlEnabled)
        {
            moveInput = context.ReadValue<Vector2>().normalized;
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    public void OnSprint(InputAction.CallbackContext context) {

        if (context.canceled) {
            currentSpeed = walkSpeed;
            Debug.Log("IM WALKING CALM");
        }else if(isGrounded) {
            currentSpeed = sprintSpeed;
            Debug.Log("IM RUNNING WILD");
        }
        
        Debug.Log("IM RUNNING WILD");
    }

    // Handle Jump-input
    // This method can be triggered through the UnityEvent in PlayerInput
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && controlEnabled)
        {
            Debug.Log("Jump!");
            
            jumpInput = true;
            jumpReleased = false;
        }

        if (context.canceled && controlEnabled)
        {
            jumpReleased = true;
            jumpInput = false;
        }
    }
}
