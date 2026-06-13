using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float gravity = -20f;
    public float jumpHeight = 1.5f;
    public float rotationSpeed = 10f;

    [Header("Camera")]
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;

    private float verticalVelocity = 0f; // Only track vertical separately
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleGravityAndJump();
        HandleMovement();
    }

    void HandleMovement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        Vector3 camRight = cameraTransform.right;

        camForward.y = 0f;
        camRight.y = 0f;

        camForward.Normalize();
        camRight.Normalize();

        Vector3 move = (camForward * v + camRight * h);

        if (move.magnitude > 1f)
            move.Normalize();

        bool running = Input.GetKey(KeyCode.LeftShift);
        float speed = running ? runSpeed : walkSpeed;

        // Combine horizontal movement with vertical velocity
        Vector3 finalMove = move * speed;
        finalMove.y = verticalVelocity;

        controller.Move(finalMove * Time.deltaTime);

        // Rotate player to face movement direction
        if (move.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }

        // Animator
        float blendSpeed = 0f;

        if (move.magnitude > 0.1f)
        {
            blendSpeed = running ? 1f : 0.5f;
        }

        if (animator != null)
            animator.SetFloat("Speed", blendSpeed);
    }

    void HandleGravityAndJump()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded)
        {
            verticalVelocity = -2f; // Small downward force to stay grounded

            // Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                if (animator != null)
                    animator.SetTrigger("Jump");
            }
        }
        else
        {
            // Apply gravity only when not grounded
            verticalVelocity += gravity * Time.deltaTime;
        }
    }
}