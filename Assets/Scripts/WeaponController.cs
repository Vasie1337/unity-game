using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public Transform firePoint;
    public float fireRate = 0.3f;
    public float damage = 10f;
    public float range = 100f;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    
    [Header("Hit Effects")]
    public GameObject hitEffect;
    public float hitEffectLifetime = 2f;
    public float impactForce = 30f;
    
    [Header("Recoil Settings")]
    public float recoilAmount = 0.1f;
    public float recoilTiltAmount = 5f; // Degrees to tilt back
    public float recoilSpeed = 10f;
    public float recoilRecoverySpeed = 5f;
    
    [Header("Weapon Sway Settings")]
    public float lookSwayAmount = 0.02f;
    public float lookSwayMaxAmount = 0.06f;
    public float lookRotationSwayAmount = 1f;
    public float movementSwayAmount = 0.02f;
    public float movementSwaySpeed = 2f;
    public float swaySmoothness = 6f;
    
    [Header("Reload Animation Settings")]
    public float reloadSpinSpeed = 720f; // Degrees per second (2 full rotations per second)
    public bool spinDuringReload = true;
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    
    [Header("Crosshair Settings")]
    public bool showCrosshair = true;
    public Color crosshairColor = new Color(1f, 1f, 1f, 0.8f);
    public float crosshairSize = 20f;
    public float crosshairThickness = 2f;
    public float crosshairGap = 5f; // Gap in the center
    
    // Private variables
    private float nextFireTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 currentRecoilPosition;
    private Vector3 currentRecoilRotation;
    private float currentReloadRotation = 0f;
    
    // References
    private Movement playerMovement;
    private Rigidbody playerRigidbody;
    
    void Start()
    {
        currentAmmo = maxAmmo;
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        
        // Find player movement component
        playerMovement = GetComponentInParent<Movement>();
        if (playerMovement != null)
        {
            playerRigidbody = playerMovement.GetComponent<Rigidbody>();
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }
    
    void Update()
    {
        if (isReloading)
            return;
            
        HandleShooting();
        HandleReload();
    }
    
    void LateUpdate()
    {
        // Apply sway and recoil in LateUpdate to ensure smooth movement
        HandleWeaponSway();
        HandleRecoil();
    }
    
    void HandleShooting()
    {
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime && currentAmmo > 0)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }
    
    void Shoot()
    {
        currentAmmo--;
        
        // Perform raycast
        if (firePoint != null)
        {
            RaycastHit hit;
            Vector3 rayOrigin = firePoint.position;
            Vector3 rayDirection = firePoint.forward;
            
            // Draw debug ray
            Debug.DrawRay(rayOrigin, rayDirection * range, Color.red, 0.1f);
            
            if (Physics.Raycast(rayOrigin, rayDirection, out hit, range))
            {
                Debug.Log($"Hit: {hit.collider.name} at distance: {hit.distance}");
                
                // Apply damage to damageable objects
                IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    damageable.TakeDamage(damage);
                }
                
                // Apply impact force to rigidbodies
                Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForceAtPosition(rayDirection * impactForce, hit.point, ForceMode.Impulse);
                }
                
                // Create hit effect at impact point
                if (hitEffect != null)
                {
                    GameObject effect = Instantiate(hitEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(effect, hitEffectLifetime);
                }
                
                // Optional: Create a visible tracer line
                CreateTracerLine(rayOrigin, hit.point);
            }
            else
            {
                // No hit - create tracer to max range
                CreateTracerLine(rayOrigin, rayOrigin + rayDirection * range);
            }
        }
        
        // Apply recoil to weapon only
        currentRecoilPosition.z -= recoilAmount; // Pull back
        currentRecoilRotation.x -= recoilTiltAmount; // Tilt up
        
        // Effects
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        
        if (audioSource != null && fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
    }
    
    void CreateTracerLine(Vector3 start, Vector3 end)
    {
        // Create a temporary line renderer for the tracer
        GameObject tracerObj = new GameObject("Tracer");
        LineRenderer lineRenderer = tracerObj.AddComponent<LineRenderer>();
        
        // Configure the line renderer
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(1f, 0.8f, 0f, 0.8f);
        lineRenderer.endColor = new Color(1f, 0.4f, 0f, 0.2f);
        
        // Set line positions
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        
        // Destroy after a short time
        Destroy(tracerObj, 0.05f);
    }
    
    void HandleReload()
    {
        // Only reload when R is pressed and we need ammo
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }

        // Only reload when Fire1 is pressed and we need ammo
        if (Input.GetButton("Fire1") && currentAmmo == 0 && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }
    
    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        currentReloadRotation = 0f;
        
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < reloadTime)
        {
            elapsedTime += Time.deltaTime;
            
            if (spinDuringReload)
            {
                currentReloadRotation = elapsedTime * reloadSpinSpeed;
            }
            
            yield return null;
        }
        
        currentReloadRotation = 0f;
        currentAmmo = maxAmmo;
        isReloading = false;
    }
    
    void HandleWeaponSway()
    {
        // Look sway (from mouse input)
        float mouseX = Input.GetAxis("Mouse X") * lookSwayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * lookSwayAmount;
        
        // Clamp look sway
        mouseX = Mathf.Clamp(mouseX, -lookSwayMaxAmount, lookSwayMaxAmount);
        mouseY = Mathf.Clamp(mouseY, -lookSwayMaxAmount, lookSwayMaxAmount);
        
        // Calculate look sway position and rotation
        Vector3 lookSwayPosition = new Vector3(-mouseX, -mouseY, 0);
        Quaternion lookSwayRotation = Quaternion.Euler(
            mouseY * lookRotationSwayAmount,
            mouseX * lookRotationSwayAmount,
            mouseX * lookRotationSwayAmount * 0.5f
        );
        
        // Movement sway (from player velocity)
        Vector3 movementSway = Vector3.zero;
        if (playerRigidbody != null)
        {
            Vector3 localVelocity = transform.InverseTransformDirection(playerRigidbody.linearVelocity);
            
            // Side-to-side sway from strafing
            movementSway.x = Mathf.Sin(Time.time * movementSwaySpeed) * localVelocity.x * movementSwayAmount;
            
            // Up-down bob from forward/backward movement
            movementSway.y = Mathf.Sin(Time.time * movementSwaySpeed * 2f) * Mathf.Abs(localVelocity.z) * movementSwayAmount;
        }
        
        // Combine all position offsets
        Vector3 targetPosition = initialPosition + lookSwayPosition + movementSway + currentRecoilPosition;
        
        // Add reload rotation
        Quaternion reloadRotation = Quaternion.Euler(-currentReloadRotation, 0, 0);
        
        // Combine all rotations
        Quaternion targetRotation = initialRotation * reloadRotation * lookSwayRotation * Quaternion.Euler(currentRecoilRotation);
        
        // Apply sway smoothly
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.deltaTime * swaySmoothness
        );
        
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            targetRotation,
            Time.deltaTime * swaySmoothness
        );
    }
    
    void HandleRecoil()
    {
        // Recover from recoil
        currentRecoilPosition = Vector3.Lerp(currentRecoilPosition, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
        currentRecoilRotation = Vector3.Lerp(currentRecoilRotation, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
    }

    // Simple crosshair
    void OnGUI()
    {
        // Draw a simple crosshair
        if (!showCrosshair)
            return;
            
        // Get screen center
        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;
        
        // Set GUI color
        GUI.color = crosshairColor;
        
        // Draw horizontal lines (left and right)
        GUI.DrawTexture(new Rect(centerX - crosshairSize - crosshairGap, centerY - crosshairThickness / 2f, crosshairSize, crosshairThickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX + crosshairGap, centerY - crosshairThickness / 2f, crosshairSize, crosshairThickness), Texture2D.whiteTexture);
        
        // Draw vertical lines (top and bottom)
        GUI.DrawTexture(new Rect(centerX - crosshairThickness / 2f, centerY - crosshairSize - crosshairGap, crosshairThickness, crosshairSize), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(centerX - crosshairThickness / 2f, centerY + crosshairGap, crosshairThickness, crosshairSize), Texture2D.whiteTexture);
        
        // Optional: Draw center dot
        // GUI.DrawTexture(new Rect(centerX - 1f, centerY - 1f, 2f, 2f), Texture2D.whiteTexture);
        
        // Reset GUI color
        GUI.color = Color.white;
    }
} 

// Simple damage interface
public interface IDamageable
{
    void TakeDamage(float damage);
} 