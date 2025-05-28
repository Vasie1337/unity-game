using UnityEngine;

public class Target : MonoBehaviour, IDamageable
{
    [Header("Target Settings")]
    public float maxHealth = 100f;
    public float currentHealth;
    
    [Header("Visual Feedback")]
    public Renderer targetRenderer;
    public Color damageColor = Color.red;
    public float colorChangeDuration = 0.1f;
    
    [Header("Death Effects")]
    public GameObject deathEffect;
    public float deathEffectDuration = 2f;
    public bool destroyOnDeath = true;
    
    private Color originalColor;
    private float colorChangeTimer = 0f;
    
    void Start()
    {
        currentHealth = maxHealth;
        
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<Renderer>();
        }
        
        if (targetRenderer != null)
        {
            originalColor = targetRenderer.material.color;
        }
    }
    
    void Update()
    {
        // Handle damage color feedback
        if (colorChangeTimer > 0)
        {
            colorChangeTimer -= Time.deltaTime;
            
            if (colorChangeTimer <= 0 && targetRenderer != null)
            {
                targetRenderer.material.color = originalColor;
            }
        }
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        // Visual feedback
        if (targetRenderer != null)
        {
            targetRenderer.material.color = damageColor;
            colorChangeTimer = colorChangeDuration;
        }
        
        // Check if dead
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        // Create death effect
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, transform.rotation);
            Destroy(effect, deathEffectDuration);
        }
        
        // Destroy or disable
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
    
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
    
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
    }
} 