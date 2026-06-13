using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform target; // Assign your Player here

    [Header("Distance Settings")]
    public float distance = 5f;
    public float minDistance = 2f;
    public float maxDistance = 10f;
    public float zoomSpeed = 2f;

    [Header("Height Settings")]
    public float heightOffset = 1.5f;

    [Header("Rotation Settings")]
    public float mouseSensitivity = 3f;
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 60f;

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
        if (!cursorLocked) return; // Stop rotating when puzzle is open

        HandleRotationInput();
        HandleZoomInput();
        HandleCameraPosition();
    }

    void HandleRotationInput()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        currentYaw += mouseX;
        currentPitch -= mouseY;

        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);
    }

    void HandleZoomInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * zoomSpeed * 10f;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }

    void HandleCameraPosition()
    {
        Vector3 targetPos = target.position + Vector3.up * heightOffset;

        Quaternion desiredRotation = Quaternion.Euler(currentPitch, currentYaw, 0f);

        Vector3 desiredDirection = desiredRotation * Vector3.back;
        Vector3 desiredPosition = targetPos + desiredDirection * currentDistance;

        // Camera collision
        float adjustedDistance = currentDistance;
        RaycastHit hit;

        if (Physics.SphereCast(targetPos, collisionRadius, desiredDirection, out hit, currentDistance, collisionLayers))
        {
            adjustedDistance = Mathf.Clamp(hit.distance, minDistance, currentDistance);
        }

        Vector3 finalPosition = targetPos + desiredDirection * adjustedDistance;

        smoothedPosition = Vector3.Lerp(smoothedPosition, finalPosition, Time.deltaTime * positionSmoothing);
        smoothedRotation = Quaternion.Lerp(smoothedRotation, desiredRotation, Time.deltaTime * rotationSmoothing);

        transform.position = smoothedPosition;
        transform.rotation = smoothedRotation;
    }

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
}