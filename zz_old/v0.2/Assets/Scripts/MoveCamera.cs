using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [Header("Sensitivity Settings")]
    public float mouseSensitivity = 100f;

    [Header("Camera Setup")]
    public Transform playerBody; // Reference to player body for horizontal rotation

    private float xRotation = 0f;

    private void Start()
    {
        // Lock and hide the cursor for FPS-style control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Read mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Vertical rotation (look up/down)
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent flipping

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Horizontal rotation (turn player body)
        playerBody.Rotate(Vector3.up * mouseX);
    }
}