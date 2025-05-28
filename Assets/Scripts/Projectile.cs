using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
    public float damage = 10f;
    public float lifeTime = 5f;
    public float impactForce = 30f;
    
    [Header("Effects")]
    public GameObject impactEffect;
    public float impactEffectLifetime = 2f;
    
    [Header("Trail")]
    public bool useTrail = true;
    public float trailTime = 0.1f;
    
    private TrailRenderer trailRenderer;
    private float spawnTime;
    
    void Start()
    {
        spawnTime = Time.time;
        
        // Add trail renderer if enabled
        if (useTrail)
        {
            trailRenderer = GetComponent<TrailRenderer>();
            if (trailRenderer == null)
            {
                trailRenderer = gameObject.AddComponent<TrailRenderer>();
                ConfigureTrailRenderer();
            }
        }
        
        // Add collider if not present
        if (GetComponent<Collider>() == null)
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.radius = 0.05f;
            col.isTrigger = true;
        }
        
        // Destroy after lifetime
        Destroy(gameObject, lifeTime);
    }
    
    void ConfigureTrailRenderer()
    {
        trailRenderer.time = trailTime;
        trailRenderer.startWidth = 0.05f;
        trailRenderer.endWidth = 0f;
        trailRenderer.material = new Material(Shader.Find("Sprites/Default"));
        trailRenderer.startColor = new Color(1f, 0.8f, 0f, 0.8f);
        trailRenderer.endColor = new Color(1f, 0.4f, 0f, 0f);
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Don't hit the shooter
        if (other.CompareTag("Player") && Time.time - spawnTime < 0.1f)
            return;
            
        // Apply damage to damageable objects
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }
        
        // Apply impact force to rigidbodies
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForceAtPosition(transform.forward * impactForce, transform.position, ForceMode.Impulse);
        }
        
        // Create impact effect
        if (impactEffect != null)
        {
            GameObject effect = Instantiate(impactEffect, transform.position, Quaternion.LookRotation(transform.forward * -1));
            Destroy(effect, impactEffectLifetime);
        }
        
        // Destroy projectile
        Destroy(gameObject);
    }
    
    void OnCollisionEnter(Collision collision)
    {
        // Handle collision same as trigger for non-trigger colliders
        OnTriggerEnter(collision.collider);
    }
}

// Simple damage interface
public interface IDamageable
{
    void TakeDamage(float damage);
} 