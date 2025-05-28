using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Header("Crosshair Settings")]
    public float crosshairSize = 20f;
    public float crosshairThickness = 2f;
    public float crosshairGap = 5f;
    public Color crosshairColor = Color.white;
    
    [Header("Dynamic Crosshair")]
    public bool dynamicCrosshair = true;
    public float movementSpread = 10f;
    public float shootingSpread = 20f;
    public float spreadSpeed = 5f;
    public float recoverSpeed = 3f;
    
    private float currentSpread = 0f;
    private Movement playerMovement;
    private WeaponController weaponController;
    private bool isMoving = false;
    private float lastShotTime = 0f;
    
    void Start()
    {
        // Find player movement component
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<Movement>();
        }
        
        // Find weapon controller
        weaponController = FindObjectOfType<WeaponController>();
    }
    
    void Update()
    {
        if (dynamicCrosshair)
        {
            UpdateDynamicCrosshair();
        }
    }
    
    void UpdateDynamicCrosshair()
    {
        // Check if moving
        isMoving = false;
        if (playerMovement != null)
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");
            isMoving = Mathf.Abs(horizontalInput) > 0.1f || Mathf.Abs(verticalInput) > 0.1f;
        }
        
        // Check if shooting
        bool isShooting = false;
        if (weaponController != null && Input.GetButton("Fire1"))
        {
            lastShotTime = Time.time;
            isShooting = true;
        }
        
        // Calculate target spread
        float targetSpread = 0f;
        if (isShooting || Time.time - lastShotTime < 0.1f)
        {
            targetSpread = shootingSpread;
        }
        else if (isMoving)
        {
            targetSpread = movementSpread;
        }
        
        // Smoothly adjust spread
        if (currentSpread < targetSpread)
        {
            currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * spreadSpeed);
        }
        else
        {
            currentSpread = Mathf.Lerp(currentSpread, targetSpread, Time.deltaTime * recoverSpeed);
        }
    }
    
    void OnGUI()
    {
        // Calculate center of screen
        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;
        
        // Calculate crosshair dimensions
        float size = crosshairSize + currentSpread;
        float gap = crosshairGap + currentSpread * 0.5f;
        
        // Set color
        GUI.color = crosshairColor;
        
        // Draw crosshair lines
        // Top line
        GUI.DrawTexture(new Rect(centerX - crosshairThickness / 2f, centerY - size - gap, crosshairThickness, size), Texture2D.whiteTexture);
        
        // Bottom line
        GUI.DrawTexture(new Rect(centerX - crosshairThickness / 2f, centerY + gap, crosshairThickness, size), Texture2D.whiteTexture);
        
        // Left line
        GUI.DrawTexture(new Rect(centerX - size - gap, centerY - crosshairThickness / 2f, size, crosshairThickness), Texture2D.whiteTexture);
        
        // Right line
        GUI.DrawTexture(new Rect(centerX + gap, centerY - crosshairThickness / 2f, size, crosshairThickness), Texture2D.whiteTexture);
        
        // Center dot (optional)
        if (crosshairGap < 2f)
        {
            GUI.DrawTexture(new Rect(centerX - 1, centerY - 1, 2, 2), Texture2D.whiteTexture);
        }
        
        // Reset color
        GUI.color = Color.white;
    }
} 