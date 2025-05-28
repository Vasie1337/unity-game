using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Mouse Sensitivity")]
    public float mouseSensitivity = 100f;
    public float sensitivityMultiplier = 1f;

    [Header("Rotation Limits")]
    public float minVerticalAngle = -90f;
    public float maxVerticalAngle = 90f;

    [Header("References")]
    public Transform playerBody;
    public Transform cameraHolder;

    private float xRotation = 0f;
    private bool cursorLocked = true;

    void Start()
    {
        LockCursor();

        if (playerBody == null)
        {
            if (transform.parent != null && transform.parent.parent != null)
            {
                playerBody = transform.parent.parent;
            }
            else
            {
                Debug.LogWarning("Player body reference not set! Please assign it in the inspector.");
            }
        }

        if (cameraHolder == null)
        {
            cameraHolder = transform.parent;
            if (cameraHolder == null)
            {
                Debug.LogWarning("Camera holder reference not set! Please assign it in the inspector.");
            }
        }
    }

    void Update()
    {
        HandleCursorLock();

        if (cursorLocked)
        {
            HandleMouseLook();
        }
    }

    void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * sensitivityMultiplier * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * sensitivityMultiplier * Time.deltaTime;

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minVerticalAngle, maxVerticalAngle);

        if (cameraHolder != null)
        {
            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }

    void HandleCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UnlockCursor();
        }

        if (Input.GetMouseButtonDown(0) && !cursorLocked)
        {
            LockCursor();
        }
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cursorLocked = true;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        cursorLocked = false;
    }
} 