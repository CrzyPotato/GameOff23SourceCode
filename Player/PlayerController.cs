using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Rigidbody rb;
    [SerializeField] private GameObject camHolder;
    [SerializeField] private Image imageCooldown;

    [Header("Variables")]
    public float speed, sensitivity, maxForce, jumpForce;
    public Vector2 move, look;
    private float lookRotation;
    private bool grounded;

    [Header("Jump Ability")]
    public float damage;
    public float damageRadius;
    [SerializeField] private LayerMask damageLayers;
    public float cooldown;
    private float nextJumpTime;
    private float imageCooldownTimer;
    public bool IsJumpCoolingDown => Time.time < nextJumpTime;
    public void StartCoolDown() => nextJumpTime = Time.time + cooldown;

    public void OnMove(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        look = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    void Start()
    {
        imageCooldown.fillAmount = 0f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        if (imageCooldownTimer > 0)
        {
            imageCooldownTimer -= Time.deltaTime;
            imageCooldown.fillAmount = imageCooldownTimer / cooldown;
        }
        else
            imageCooldown.fillAmount = 0f;


        Move();
    }

    void LateUpdate()
    {
        Rotate();
    }

    private void Move()
    {
        // Find Target velocity
        Vector3 currentVelocity = rb.velocity;
        Vector3 targetVelocity = new Vector3(move.x, 0, move.y);
        targetVelocity *= speed;

        // Align direction
        targetVelocity = transform.TransformDirection(targetVelocity);

        // Calculate forces
        Vector3 velocityChange = (targetVelocity - currentVelocity);
        velocityChange = new Vector3 (velocityChange.x, 0, velocityChange.z);

        // Limit force
        Vector3.ClampMagnitude(velocityChange, maxForce);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);
    }

    private void Rotate()
    {
        // Turn
        transform.Rotate(Vector3.up * look.x * sensitivity);

        // Look
        lookRotation += (-look.y * sensitivity);
        lookRotation = Mathf.Clamp(lookRotation, -90, 90);
        camHolder.transform.eulerAngles = new Vector3(lookRotation, camHolder.transform.eulerAngles.y, camHolder.transform.eulerAngles.z);
    }

    private void Jump()
    {
        if (IsJumpCoolingDown) 
            return;

        Vector3 jumpForces = Vector3.zero;

        if (grounded)
        {
            jumpForces = Vector3.up * jumpForce;
        }

        rb.AddForce(jumpForces, ForceMode.VelocityChange);

        StartCoolDown();
        imageCooldownTimer = cooldown;
    }

    public void SetGrounded(bool state)
    {
        grounded = state;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Vector3 radiusDamagePos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(radiusDamagePos, damageRadius, damageLayers);
        int i = 0;
        while (i < colliders.Length)
        {
            Collider hit = colliders[i];
            hit.GetComponent<Health>().DealDamage(damage);
            Debug.Log(hit.name);
            i++;
        }
    }
}
