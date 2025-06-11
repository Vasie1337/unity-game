using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Jump Pad Settings")]
    public float jumpForce = 20f;
    public bool useVelocityChange = true;
    public bool resetVerticalVelocity = true;
    
    [Header("Effects")]
    public ParticleSystem jumpEffect;
    public AudioSource jumpSound;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has a Rigidbody
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            ApplyJumpForce(rb);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Alternative collision detection for non-trigger colliders
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            ApplyJumpForce(rb);
        }
    }

    private void ApplyJumpForce(Rigidbody rb)
    {
        // Reset vertical velocity if enabled (for consistent jump height)
        if (resetVerticalVelocity)
        {
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0f;
            rb.linearVelocity = velocity;
        }

        // Apply upward force
        ForceMode forceMode = useVelocityChange ? ForceMode.VelocityChange : ForceMode.Impulse;
        rb.AddForce(Vector3.up * jumpForce, forceMode);

        // Play effects
        PlayEffects();
    }

    private void PlayEffects()
    {
        // Play particle effect
        if (jumpEffect != null)
        {
            jumpEffect.Play();
        }

        // Play sound effect
        if (jumpSound != null)
        {
            jumpSound.Play();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize jump force in the scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.up * jumpForce * 0.1f);
        
        // Draw a sphere to represent the jump pad area
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
