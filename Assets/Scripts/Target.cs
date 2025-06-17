using UnityEngine;

public class Target : MonoBehaviour, IDamageable
{
    [Header("Target Settings")]
    public float maxHealth = 30f;
    public float currentHealth;
    
    [Header("Visual Feedback")]
    private Renderer targetRenderer;
    private Color damageColor = Color.blue;
    private float colorChangeDuration = 0.2f;
    
    [Header("Death Effects")]
    public GameObject deathEffect;
    private float deathEffectDuration = 2f;
    private bool destroyOnDeath = true;
    
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
        
        if (targetRenderer != null)
        {
            targetRenderer.material.color = damageColor;
            colorChangeTimer = colorChangeDuration;
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    void Die()
    {
        if (deathEffect != null)
        {
            GameObject effect = Instantiate(deathEffect, transform.position, transform.rotation);
            Destroy(effect, deathEffectDuration);
        }
        
        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
} 