using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target;
    public Transform playerBody;

    [Header("FPP Hand Renderers")]
    public Renderer[] handRenderers; // Assign ONLY hand renderers here in Inspector

    [Header("First Person Settings")]
    public Vector3 firstPersonOffset = new Vector3(0f, 1.7f, 0f);

    [Header("Distance Settings")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 2f;

    [Header("Height Settings")]
    public float heightOffset = 1.5f;

    [Header("Rotation Settings")]
    public float mouseSensitivity = 3f;
    public float minVerticalAngle = -80f;
    public float maxVerticalAngle = 80f;

    [Header("Smoothing")]
    public float rotationSmoothing = 10f;
    public float positionSmoothing = 10f;

    [Header("Collision")]
    public float collisionRadius = 0.3f;
    public LayerMask collisionLayers;

    private float currentYaw = 0f;
    private float currentPitch = 20f;
    private float currentDistance;

    private Vector3 smoothedPosition;
    private Quaternion smoothedRotation;

    private bool cursorLocked = true;
    private bool isFirstPerson = false;

    void Start()
    {
        currentDistance = distance;
        smoothedPosition = transform.position;
        smoothedRotation = transform.rotation;

        LockCursor();

        if (target == null)
            Debug.LogError("ThirdPersonCamera: No target assigned!");
    }

    void LateUpdate()
    {
        if (target == null) return;
        if (!cursorLocked) return;

        HandleViewToggle();
        HandleRotationInput();

        if (isFirstPerson)
            HandleFirstPersonCamera();
        else
        {
            HandleZoomInput();
            HandleThirdPersonCamera();
        }
    }

    // ------------------- VIEW TOGGLE -------------------

    void HandleViewToggle()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;

            if (isFirstPerson)
            {
                // Hide full body, show only hands
                SetBodyVisible(false);
                SetHandsVisible(true);
                Debug.Log("Switched to First Person");
            }
            else
            {
                // Show full body, hide hands overlay
                SetBodyVisible(true);
                SetHandsVisible(false);
                Debug.Log("Switched to Third Person");
            }
        }
    }

    // Disable ALL renderers on player body
    void SetBodyVisible(bool visible)
    {
        if (playerBody == null) return;

        Renderer[] renderers = playerBody.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            r.enabled = visible;
        }
    }

    // Enable ONLY hand renderers
    void SetHandsVisible(bool visible)
    {
        if (handRenderers == null) return;

        foreach (Renderer r in handRenderers)
        {
            if (r != null)
                r.enabled = visible;
        }
    }

    // ------------------- ROTATION -------------------

    void HandleRotationInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentYaw += mouseX;
        currentPitch -= mouseY;

        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        if (isFirstPerson && target != null)
            target.rotation = Quaternion.Euler(0f, currentYaw, 0f);
    }

    // ------------------- ZOOM (TPP only) -------------------

    void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * zoomSpeed * 10f;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }

    // ------------------- FIRST PERSON CAMERA -------------------

    void HandleFirstPersonCamera()
    {
        Vector3 eyePosition = target.position + Vector3.up * firstPersonOffset.y;
        transform.position = eyePosition;
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    // ------------------- THIRD PERSON CAMERA -------------------

    void HandleThirdPersonCamera()
    {
        Vector3 targetPos = target.position + Vector3.up * heightOffset;

        Quaternion desiredRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 desiredDirection = desiredRotation * Vector3.back;

        float adjustedDistance = currentDistance;
        RaycastHit hit;

        if (Physics.SphereCast(targetPos, collisionRadius, desiredDirection, out hit, currentDistance, collisionLayers))
            adjustedDistance = Mathf.Clamp(hit.distance, minDistance, currentDistance);

        Vector3 finalPosition = targetPos + desiredDirection * adjustedDistance;

        smoothedPosition = Vector3.Lerp(smoothedPosition, finalPosition, Time.deltaTime * positionSmoothing);
        smoothedRotation = Quaternion.Lerp(smoothedRotation, desiredRotation, Time.deltaTime * rotationSmoothing);

        transform.position = smoothedPosition;
        transform.rotation = smoothedRotation;
    }

    // ------------------- CURSOR LOCK -------------------

    public void UnlockCursor()
    {
        cursorLocked = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LockCursor()
    {
        cursorLocked = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // ------------------- PUBLIC GETTER -------------------

    public bool IsFirstPerson()
    {
        return isFirstPerson;
    }
}