/*using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovePlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private Transform cameraTransform;

    private Vector2 moveInput;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!cameraTransform && Camera.main)
            cameraTransform = Camera.main.transform;
    }

    public void OnMove(InputValue value) => moveInput = value.Get<Vector2>();

    private void FixedUpdate()
    {
        if (moveInput == Vector2.zero || !cameraTransform) return;

        // Get flattened camera directions
        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right   = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        // Calculate desired direction
        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;

        // Move and rotate
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, Quaternion.LookRotation(moveDir), rotationSpeed * Time.fixedDeltaTime));
    }
}*/
/*using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovePlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;       // WASD movement speed
    [SerializeField] private float turnSpeed = 5f;       // Turning speed for keyboard input
    [SerializeField] private float mouseSensitivity = 2f; // Mouse look sensitivity

    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;

    private Vector2 moveInput;
    private Vector2 lookInput;
    private Rigidbody rb;

    private float pitch = 0f; // vertical rotation

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (!cameraTransform && Camera.main)
            cameraTransform = Camera.main.transform;
    }

    // Input callbacks for Unity Events
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        Cursor.visible = false;
        if (hasFocus)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }   

    private void FixedUpdate()
    {
        if (!cameraTransform) return;

        // --- Movement ---
        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right   = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;

        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        // --- Rotation ---
        if (moveDir != Vector3.zero)
        {
            // Smooth rotation toward movement direction (keyboard)
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime));
        }

        // --- Mouse Look ---
        float yaw = lookInput.x * mouseSensitivity;
        float pitchDelta = -lookInput.y * mouseSensitivity;
        pitch = Mathf.Clamp(pitch + pitchDelta, -80f, 80f);

        transform.Rotate(Vector3.up, yaw, Space.World);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);

        // --- Animator ---
        if (animator)
            animator.SetFloat("Speed", moveInput.magnitude);
    }
}*/
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class MovePlayer : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float mouseSensitivity = 0.5f;
    [SerializeField] private float gamepadLookSensitivity = 120f;
    [SerializeField] private float rotationSmooth = 10f;

    [Header("Camera")]
    [SerializeField] private Transform cameraTransform;

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Rigidbody rb;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float pitch = 0f;

    private PlayerInput playerInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // stops tipping
        playerInput = GetComponent<PlayerInput>();

        if (!cameraTransform && Camera.main)
            cameraTransform = Camera.main.transform;
    }

    // Input System Callbacks
    public void OnMove(InputAction.CallbackContext ctx)
        => moveInput = ctx.ReadValue<Vector2>();

    public void OnLook(InputAction.CallbackContext ctx)
        => lookInput = ctx.ReadValue<Vector2>();


    private void Update()
    {
        HandleLook();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleLook()
    {
        if (!cameraTransform) return;

        // Determine control scheme: KeyboardMouse or Gamepad
        string scheme = playerInput.currentControlScheme;

        float yaw;
        float pitchDelta;

        if (scheme == "Keyboard&Mouse")  // Player 1
        {
            if (!Application.isFocused) return;

            Cursor.visible = false;
            yaw = lookInput.x * mouseSensitivity;
            pitchDelta = -lookInput.y * mouseSensitivity;
        }
        else // Gamepad (Player 2)
        {
            yaw = lookInput.x * gamepadLookSensitivity * Time.deltaTime;
            pitchDelta = -lookInput.y * gamepadLookSensitivity * Time.deltaTime;
        }

        // Rotate player horizontally
        transform.Rotate(Vector3.up, yaw);

        // Clamp camera pitch
        pitch = Mathf.Clamp(pitch + pitchDelta, -80f, 80f);
        cameraTransform.localEulerAngles = new Vector3(pitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // W/S = forward/back   A/D = strafe
        Vector3 moveDir = (forward * moveInput.y + right * moveInput.x).normalized;

        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);

        // Animator
        if (animator)
            animator.SetFloat("Speed", moveInput.magnitude);
    }
}
