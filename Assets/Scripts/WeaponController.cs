using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float fireRate = 0.3f;
    public float projectileSpeed = 50f;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    
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
    
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip fireSound;
    public AudioClip reloadSound;
    
    [Header("Effects")]
    public ParticleSystem muzzleFlash;
    
    // Private variables
    private float nextFireTime = 0f;
    private int currentAmmo;
    private bool isReloading = false;
    private Vector3 initialPosition;
    private Quaternion initialRotation;
    private Vector3 currentRecoilPosition;
    private Vector3 currentRecoilRotation;
    
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
        
        // Create projectile
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            
            if (rb == null)
            {
                rb = projectile.AddComponent<Rigidbody>();
            }
            
            rb.useGravity = false;
            rb.linearVelocity = firePoint.forward * projectileSpeed;
            
            // Add projectile component if it doesn't exist
            if (projectile.GetComponent<Projectile>() == null)
            {
                projectile.AddComponent<Projectile>();
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
    
    void HandleReload()
    {
        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo && !isReloading)
        {
            StartCoroutine(Reload());
        }
    }
    
    System.Collections.IEnumerator Reload()
    {
        isReloading = true;
        
        if (audioSource != null && reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
        
        yield return new WaitForSeconds(reloadTime);
        
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
        
        // Combine all rotations
        Quaternion targetRotation = initialRotation * lookSwayRotation * Quaternion.Euler(currentRecoilRotation);
        
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
    
    void OnGUI()
    {
        // Simple ammo display
        if (!isReloading)
        {
            GUI.Label(new Rect(10, Screen.height - 40, 200, 30), $"Ammo: {currentAmmo}/{maxAmmo}");
        }
        else
        {
            GUI.Label(new Rect(10, Screen.height - 40, 200, 30), "Reloading...");
        }
    }
} 