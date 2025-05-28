using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 20f;
    public float sprintSpeed = 30f;
    public float groundAcceleration = 15f;
    public float groundDeceleration = 15f;
    public float airAcceleration = 8f;
    public float airDeceleration = 2f;
    public float maxVelocity = 35f;
    public float friction = 10f;

    [Header("Jump Settings")]
    public float jumpHeight = 3f;
    public float jumpCooldown = 0.2f;
    public float gravityScale = 3f;
    public float fallMultiplier = 1.5f;

    [Header("Ground Check")]
    public float groundCheckRadius = 0.3f;
    public LayerMask groundLayer = -1;
    public float groundCheckOffset = 0.1f;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;

    // Components
    private Rigidbody rb;
    private CapsuleCollider col;

    // State
    private Vector3 moveInput;
    private bool isGrounded;
    private bool canJump = true;
    private float currentSpeed;

    // Ground check
    private Vector3 groundCheckPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();
        
        // Configure rigidbody for responsive movement
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        
        // Set custom gravity
        Physics.gravity = new Vector3(0, -9.81f * gravityScale, 0);
    }

    void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Calculate move direction relative to camera
        Transform cam = Camera.main.transform;
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        
        moveInput = (forward * vertical + right * horizontal).normalized;
        
        // Sprint check
        currentSpeed = Input.GetKey(sprintKey) ? sprintSpeed : moveSpeed;
        
        // Jump
        if (Input.GetKeyDown(jumpKey) && isGrounded && canJump)
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        CheckGround();
        ApplyMovement();
        ApplyFriction();
        ControlVelocity();
        ApplyGravity();
    }

    void CheckGround()
    {
        // Calculate ground check position
        float offset = col.height / 2f - col.radius + groundCheckOffset;
        groundCheckPos = transform.position + Vector3.down * offset;
        
        // Perform ground check
        isGrounded = Physics.CheckSphere(groundCheckPos, groundCheckRadius, groundLayer);
    }

    void ApplyMovement()
    {
        // Get current velocity
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        
        // Calculate desired velocity
        Vector3 targetVelocity = moveInput * currentSpeed;
        
        // Calculate velocity difference
        Vector3 velocityDiff = targetVelocity - horizontalVelocity;
        
        // Calculate acceleration
        float accel = isGrounded ? groundAcceleration : airAcceleration;
        float decel = isGrounded ? groundDeceleration : airDeceleration;
        
        // Apply acceleration or deceleration
        float acceleration = (Vector3.Dot(horizontalVelocity, targetVelocity) > 0) ? accel : decel;
        
        // Calculate force
        Vector3 moveForce = velocityDiff * acceleration;
        
        // Apply force
        rb.AddForce(moveForce, ForceMode.Force);
    }

    void ApplyFriction()
    {
        // Only apply friction when grounded and not moving
        if (isGrounded && moveInput.magnitude < 0.01f)
        {
            Vector3 velocity = rb.linearVelocity;
            Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
            
            // Apply friction force
            Vector3 frictionForce = -horizontalVelocity * friction;
            rb.AddForce(frictionForce, ForceMode.Force);
        }
    }

    void ControlVelocity()
    {
        // Limit horizontal velocity
        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVelocity = new Vector3(velocity.x, 0, velocity.z);
        
        if (horizontalVelocity.magnitude > maxVelocity)
        {
            horizontalVelocity = horizontalVelocity.normalized * maxVelocity;
            rb.linearVelocity = new Vector3(horizontalVelocity.x, velocity.y, horizontalVelocity.z);
        }
    }

    void ApplyGravity()
    {
        // Apply extra gravity when falling for better game feel
        if (rb.linearVelocity.y < 0 && !isGrounded)
        {
            rb.AddForce(Vector3.down * fallMultiplier * 10f, ForceMode.Force);
        }
    }

    void Jump()
    {
        // Reset vertical velocity
        Vector3 velocity = rb.linearVelocity;
        velocity.y = 0f;
        rb.linearVelocity = velocity;
        
        // Calculate jump velocity using physics formula: v = sqrt(2 * g * h)
        float jumpVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpHeight);
        
        // Apply jump impulse
        rb.AddForce(Vector3.up * jumpVelocity, ForceMode.VelocityChange);
        
        // Start jump cooldown
        canJump = false;
        Invoke(nameof(ResetJump), jumpCooldown);
    }

    void ResetJump()
    {
        canJump = true;
    }

    void OnDrawGizmosSelected()
    {
        // Initialize components if needed
        if (col == null) col = GetComponent<CapsuleCollider>();
        
        // Draw ground check sphere
        float offset = col.height / 2f - col.radius + groundCheckOffset;
        Vector3 checkPos = transform.position + Vector3.down * offset;
        
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(checkPos, groundCheckRadius);
        
        // Draw velocity vector
        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, rb.linearVelocity * 0.1f);
        }
    }
}