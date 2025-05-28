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
    public float recoilSpeed = 10f;
    public float recoilRecoverySpeed = 5f;
    public Vector3 recoilPattern = new Vector3(-0.1f, 0.1f, 0.05f);
    
    [Header("Weapon Sway Settings")]
    public float swayAmount = 0.02f;
    public float swayMaxAmount = 0.06f;
    public float swaySmoothness = 6f;
    public float rotationSwayAmount = 1f;
    
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
    private Vector3 currentRecoil;
    private Vector3 targetRecoil;
    
    // References
    private Camera playerCamera;
    private CameraController cameraController;
    
    void Start()
    {
        currentAmmo = maxAmmo;
        initialPosition = transform.localPosition;
        initialRotation = transform.localRotation;
        
        playerCamera = Camera.main;
        if (playerCamera != null)
        {
            cameraController = playerCamera.GetComponent<CameraController>();
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
        
        // Apply recoil
        targetRecoil += new Vector3(
            Random.Range(-recoilPattern.x, recoilPattern.x),
            Random.Range(0, recoilPattern.y),
            Random.Range(-recoilPattern.z, recoilPattern.z)
        ) * recoilAmount;
        
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
        if (cameraController == null)
            return;
            
        // Get mouse input
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;
        
        // Clamp sway
        mouseX = Mathf.Clamp(mouseX, -swayMaxAmount, swayMaxAmount);
        mouseY = Mathf.Clamp(mouseY, -swayMaxAmount, swayMaxAmount);
        
        // Calculate target position
        Vector3 targetPosition = new Vector3(-mouseX, -mouseY, 0);
        
        // Calculate target rotation
        Quaternion targetRotation = Quaternion.Euler(
            mouseY * rotationSwayAmount,
            mouseX * rotationSwayAmount,
            mouseX * rotationSwayAmount * 0.5f
        );
        
        // Apply sway smoothly
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            initialPosition + targetPosition + currentRecoil,
            Time.deltaTime * swaySmoothness
        );
        
        transform.localRotation = Quaternion.Lerp(
            transform.localRotation,
            initialRotation * targetRotation,
            Time.deltaTime * swaySmoothness
        );
    }
    
    void HandleRecoil()
    {
        // Apply recoil
        currentRecoil = Vector3.Lerp(currentRecoil, targetRecoil, Time.deltaTime * recoilSpeed);
        
        // Recover from recoil
        targetRecoil = Vector3.Lerp(targetRecoil, Vector3.zero, Time.deltaTime * recoilRecoverySpeed);
        
        // Apply recoil to camera if available
        if (cameraController != null && playerCamera != null)
        {
            float recoilX = -currentRecoil.y * 20f; // Vertical recoil
            float recoilY = currentRecoil.x * 20f;  // Horizontal recoil
            
            playerCamera.transform.localRotation *= Quaternion.Euler(recoilX * Time.deltaTime, recoilY * Time.deltaTime, 0);
        }
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