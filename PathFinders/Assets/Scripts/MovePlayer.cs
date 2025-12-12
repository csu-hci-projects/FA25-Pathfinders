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
