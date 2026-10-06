using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Lab03PlayerController : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference sprintAction;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 1.5f;
    [SerializeField] private float jumpImpulse = 6f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float deceleration = 25f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private LayerMask groundLayers;

    [Header("Feedback")]
    [SerializeField] private TrailRenderer sprintTrail;

    private Rigidbody rb;
    private Vector2 moveInput;
    private bool jumpRequested;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (sprintTrail != null)
        {
            sprintTrail.emitting = false;
        }
    }

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
        sprintAction.action.Enable();

        jumpAction.action.performed += OnJump;
    }

    private void OnDisable()
    {
        jumpAction.action.performed -= OnJump;

        moveAction.action.Disable();
        jumpAction.action.Disable();
        sprintAction.action.Disable();

        if (sprintTrail != null)
        {
            sprintTrail.emitting = false;
        }
    }

    private void Update()
    {
        moveInput = moveAction.action.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Vector3 direction =
            new Vector3(moveInput.x, 0f, moveInput.y);

        if (direction.sqrMagnitude > 1f)
        {
            direction.Normalize();
        }

        bool isMoving = direction.sqrMagnitude > 0.001f;
        bool isSprinting =
            sprintAction.action.IsPressed() && isMoving;

        float activeSpeed = isSprinting
            ? moveSpeed * sprintMultiplier
            : moveSpeed;

        Vector3 targetHorizontal = direction * activeSpeed;
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 currentHorizontal =
            new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        float rate = targetHorizontal.sqrMagnitude > 0.001f
            ? acceleration
            : deceleration;

        Vector3 nextHorizontal = Vector3.MoveTowards(
            currentHorizontal,
            targetHorizontal,
            rate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector3(
            nextHorizontal.x,
            currentVelocity.y,
            nextHorizontal.z);

        if (sprintTrail != null)
        {
            sprintTrail.emitting = isSprinting;
        }

        if (jumpRequested && IsGrounded())
        {
            rb.AddForce(
                Vector3.up * jumpImpulse,
                ForceMode.Impulse);
        }

        jumpRequested = false;
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        jumpRequested = true;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(
            transform.position,
            Vector3.down,
            groundCheckDistance,
            groundLayers);
    }
}