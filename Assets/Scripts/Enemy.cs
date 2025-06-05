using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float health = 100f;
    public float moveSpeed = 3f;
    public float rotationSpeed = 5f;
    
    [Header("Combat Settings")]
    public float detectionRange = 20f;
    public float attackRange = 15f;
    public float fireRate = 1f;
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public float bulletDamage = 10f;
    public float bulletLifetime = 5f;
    
    [Header("Targeting")]
    public LayerMask obstacleLayer = -1;
    public float targetingHeight = 1.5f; // Height offset for aiming at player center
    
    [Header("Visual Feedback")]
    public Renderer enemyRenderer;
    public Color alertColor = Color.red;
    public Color normalColor = Color.white;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    
    // Private variables
    private Transform player;
    private float nextFireTime = 0f;
    private bool hasLineOfSight = false;
    private bool isAlerted = false;
    
    void Start()
    {
        // Find player by tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("No GameObject with tag 'Player' found!");
        }
        
        // Get components
        if (enemyRenderer == null)
        {
            enemyRenderer = GetComponent<Renderer>();
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Set initial color
        if (enemyRenderer != null)
        {
            normalColor = enemyRenderer.material.color;
        }
        
        // Create fire point if not assigned
        if (firePoint == null)
        {
            GameObject firePointObj = new GameObject("FirePoint");
            firePointObj.transform.parent = transform;
            firePointObj.transform.localPosition = new Vector3(0, 0.5f, 0.5f);
            firePoint = firePointObj.transform;
        }
    }
    
    void Update()
    {
        if (player == null)
            return;
            
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        // Check if player is in detection range
        if (distanceToPlayer <= detectionRange)
        {
            // Check line of sight
            CheckLineOfSight();
            
            if (hasLineOfSight)
            {
                // Alert state
                if (!isAlerted)
                {
                    isAlerted = true;
                    if (enemyRenderer != null)
                    {
                        enemyRenderer.material.color = alertColor;
                    }
                }
                
                // Rotate to face player
                RotateTowardsPlayer();
                
                // Attack if in range
                if (distanceToPlayer <= attackRange && Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + fireRate;
                }
                
                // Move towards player if too far
                if (distanceToPlayer > attackRange * 0.8f)
                {
                    MoveTowardsPlayer();
                }
            }
            else
            {
                // Lost sight of player
                if (isAlerted)
                {
                    isAlerted = false;
                    if (enemyRenderer != null)
                    {
                        enemyRenderer.material.color = normalColor;
                    }
                }
            }
        }
        else
        {
            // Player out of range
            if (isAlerted)
            {
                isAlerted = false;
                if (enemyRenderer != null)
                {
                    enemyRenderer.material.color = normalColor;
                }
            }
        }
    }
    
    void CheckLineOfSight()
    {
        Vector3 directionToPlayer = (player.position + Vector3.up * targetingHeight) - firePoint.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        RaycastHit hit;
        if (Physics.Raycast(firePoint.position, directionToPlayer.normalized, out hit, distanceToPlayer, obstacleLayer))
        {
            // Check if we hit the player
            hasLineOfSight = hit.collider.CompareTag("Player");
        }
        else
        {
            // No obstacles between enemy and player
            hasLineOfSight = true;
        }
        
        // Debug visualization
        Debug.DrawRay(firePoint.position, directionToPlayer, hasLineOfSight ? Color.green : Color.red);
    }
    
    void RotateTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep rotation on horizontal plane
        
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    void MoveTowardsPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0; // Keep movement on horizontal plane
        
        transform.position += direction * moveSpeed * Time.deltaTime;
    }
    
    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("Bullet prefab or fire point not assigned!");
            return;
        }
        
        // Calculate direction to player with prediction
        Vector3 targetPosition = player.position + Vector3.up * targetingHeight;
        
        // Optional: Add prediction based on player velocity
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb != null)
        {
            float timeToTarget = Vector3.Distance(firePoint.position, targetPosition) / bulletSpeed;
            targetPosition += playerRb.linearVelocity * timeToTarget * 0.5f; // Partial prediction
        }
        
        Vector3 direction = (targetPosition - firePoint.position).normalized;
        
        // Create bullet
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        
        // Set up bullet component
        EnemyBullet bulletScript = bullet.GetComponent<EnemyBullet>();
        if (bulletScript == null)
        {
            bulletScript = bullet.AddComponent<EnemyBullet>();
        }
        
        bulletScript.Initialize(direction * bulletSpeed, bulletDamage, bulletLifetime);
        
        // Play shoot sound
        if (audioSource != null && shootSound != null)
        {
            audioSource.PlayOneShot(shootSound);
        }
    }
    
    public void TakeDamage(float damage)
    {
        health -= damage;
        
        if (health <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        // Add death effects here if needed
        Destroy(gameObject);
    }
    
    // Visualize ranges in editor
    void OnDrawGizmosSelected()
    {
        // Detection range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}

// Bullet class for enemy projectiles
public class EnemyBullet : MonoBehaviour
{
    private Vector3 velocity;
    private float damage;
    private float lifetime;
    private float spawnTime;
    
    [Header("Bullet Settings")]
    public GameObject hitEffect;
    public float hitEffectDuration = 2f;
    
    void Start()
    {
        spawnTime = Time.time;
        
        // Add a trail renderer for visual effect
        TrailRenderer trail = GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = gameObject.AddComponent<TrailRenderer>();
            trail.time = 0.2f;
            trail.startWidth = 0.1f;
            trail.endWidth = 0.02f;
            trail.material = new Material(Shader.Find("Sprites/Default"));
            trail.startColor = new Color(1f, 0.5f, 0f, 1f);
            trail.endColor = new Color(1f, 0.2f, 0f, 0f);
        }
    }
    
    public void Initialize(Vector3 velocity, float damage, float lifetime)
    {
        this.velocity = velocity;
        this.damage = damage;
        this.lifetime = lifetime;
        
        // Set up rigidbody if not present
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.useGravity = false;
        rb.linearVelocity = velocity;
        
        // Set up collider if not present
        if (GetComponent<Collider>() == null)
        {
            SphereCollider col = gameObject.AddComponent<SphereCollider>();
            col.radius = 0.1f;
            col.isTrigger = true;
        }
    }
    
    void Update()
    {
        // Destroy bullet after lifetime
        if (Time.time - spawnTime > lifetime)
        {
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        // Don't hit the enemy that shot this bullet
        if (other.CompareTag("Enemy"))
            return;
            
        // Apply damage to player
        if (other.CompareTag("Player"))
        {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(damage);
            }
        }
        
        // Create hit effect
        if (hitEffect != null)
        {
            GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(effect, hitEffectDuration);
        }
        
        // Destroy bullet
        Destroy(gameObject);
    }
}
