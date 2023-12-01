using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLocomotion : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 10.0f;
    public float rotationSpeed = 100.0f;
    
    [Header("Variables")]
    // Variables for clamping vertical rotation
    public float upLimit = 80.0f;
    public float downLimit = -80.0f;
    private float verticalRotation = 0f;

    [Header("Gravity and Jump Ability")]
    public float jumpForce = 10f;
    public float cooldown = 2f;
    public float radius = 5f;
    public float damage = 10f;

    [SerializeField] private CharacterController controller;
    [SerializeField] private float gravityMultiplier = 3.0f;
    private float lastJumpTime;
    private bool canJump;

    private float gravity = -9.81f;
    private float velocity;
    private Vector3 direction;

    // References
    private Vector2 move;
    private Vector2 rotate;
    private CharacterController characterController;
    private Camera mainCam;
    
    void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCam = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnMove(InputValue value)
    {
        move = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        rotate = value.Get<Vector2>();
    }

    void Update()
    {
        ApplyMovement();
        ApplyRotation();
        ApplyGravity();
    }

    private void ApplyRotation()
    {
        transform.Rotate(0, rotate.x * rotationSpeed * Time.deltaTime, 0);

        // Rotate the camera vertically and clamp it
        verticalRotation -= rotate.y * rotationSpeed * Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, downLimit, upLimit);
        Camera.main.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);

        direction = mainCam.transform.TransformDirection(direction);

        var targetRotation = Quaternion.LookRotation(direction, Vector3.up);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnJump(InputValue value)
    {
        if (!IsGrounded())
            return;

        /*if (!canJump)
            return;*/

        velocity += jumpForce;
        Debug.Log("Jump!");
    }

    private void ApplyMovement()
    {
        characterController.Move(direction * speed * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (IsGrounded() && velocity < 0.0f)
        {
            velocity = -1.0f;
        }
        else
        {
            velocity += gravity * gravityMultiplier * Time.deltaTime;
        }

        direction.y = velocity;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag("Enemy"))
                {
                    collider.gameObject.GetComponent<Health>().DealDamage(damage);
                }
            }
        }
    }

    private bool IsGrounded() => characterController.isGrounded;
}