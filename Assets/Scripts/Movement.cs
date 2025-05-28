using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float jumpForce = 8f;
    public float airMultiplier = 0.4f;
    public float groundDrag = 5f;
    public float airDrag = 1f;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask groundMask = 1;
    public float groundCheckDistance = 0.4f;

    [Header("Slope Handling")]
    public float maxSlopeAngle = 40f;

    [Header("Input")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode runKey = KeyCode.LeftShift;

    // Private variables
    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private bool grounded;
    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;

    private RaycastHit slopeHit;
    private bool exitingSlope;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        CapsuleCollider capsule = GetComponent<CapsuleCollider>();
        capsule.height = playerHeight;
        capsule.center = new Vector3(0, playerHeight / 2, 0);
    }

    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + groundCheckDistance, groundMask);

        GetInput();

        HandleDrag();

        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    void HandleDrag()
    {
        if (grounded)
            rb.linearDamping = groundDrag;
        else
            rb.linearDamping = airDrag;
    }

    void MovePlayer()
    {
        Transform camTransform = Camera.main.transform;
        Vector3 forward = camTransform.forward;
        Vector3 right = camTransform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        moveDirection = forward * verticalInput + right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * GetCurrentSpeed() * 20f, ForceMode.Force);

            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
        else if (grounded)
        {
            rb.AddForce(moveDirection.normalized * GetCurrentSpeed() * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * GetCurrentSpeed() * 10f * airMultiplier, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    float GetCurrentSpeed()
    {
        if (Input.GetKey(runKey) && grounded)
            return runSpeed;
        else
            return walkSpeed;
    }

    void Jump()
    {
        exitingSlope = true;

        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
    }

    bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + groundCheckDistance));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + Vector3.up * playerHeight / 2, new Vector3(1f, playerHeight, 1f));
    }
}