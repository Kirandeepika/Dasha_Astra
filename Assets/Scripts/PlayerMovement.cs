using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    private CharacterController controller;
    private Animator animator;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(h, 0, v);

        bool running = Input.GetKey(KeyCode.LeftShift);

        float speed = running ? runSpeed : walkSpeed;

        if (move.magnitude > 1)
            move.Normalize();

        controller.Move(move * speed * Time.deltaTime);

        if (move != Vector3.zero)
        {
            transform.forward = move;
        }

        float blendSpeed = 0f;

        if (move.magnitude > 0.1f)
        {
            blendSpeed = running ? 1f : 0.5f;
        }

        animator.SetFloat("Speed", blendSpeed);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
        }
    }
}