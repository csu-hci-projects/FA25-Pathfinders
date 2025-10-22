using UnityEngine;
using System.Collections;

public class DualButtonDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public Transform leftDoor;
    public Transform rightDoor;

    [Header("Movement Settings")]
    public float slideDistance = 5f;   // How far each door slides
    public float moveSpeed = 2f;       // Speed of sliding

    private Vector3 leftClosedPos;
    private Vector3 rightClosedPos;
    private Vector3 leftOpenPos;
    private Vector3 rightOpenPos;

    private bool button1Pressed = false;
    private bool button2Pressed = false;
    private bool isOpen = false;
    private bool permanentlyOpen = false;

    private Coroutine doorRoutine;

    private void Start()
    {
        if (!leftDoor || !rightDoor)
        {
            Debug.LogError("DualButtonDoor: Assign both door halves!");
            return;
        }

        // Record starting positions
        leftClosedPos = leftDoor.position;
        rightClosedPos = rightDoor.position;

        // Define open positions (move each door opposite directions along X)
        leftOpenPos = leftClosedPos + Vector3.left * slideDistance;
        rightOpenPos = rightClosedPos + Vector3.right * slideDistance;
    }

    public void ButtonPressed(int id)
    {
        if (permanentlyOpen) return; // Ignore if already unlocked

        if (id == 1) button1Pressed = true;
        else if (id == 2) button2Pressed = true;
        else Debug.LogWarning("DualButtonDoor: Unknown button id " + id);

        UpdateDoorState();
    }

    public void ButtonReleased(int id)
    {
        if (permanentlyOpen) return; // Ignore once unlocked

        if (id == 1) button1Pressed = false;
        else if (id == 2) button2Pressed = false;
        else Debug.LogWarning("DualButtonDoor: Unknown button id " + id);
    }

    private void UpdateDoorState()
    {
        // Check if both buttons pressed simultaneously
        if (button1Pressed && button2Pressed && !permanentlyOpen)
        {
            permanentlyOpen = true;
            StartDoorMovement(true); // open permanently
        }
    }

    private void StartDoorMovement(bool open)
    {
        if (doorRoutine != null)
            StopCoroutine(doorRoutine);

        doorRoutine = StartCoroutine(MoveDoors(open));
    }

    private IEnumerator MoveDoors(bool opening)
    {
        Vector3 lStart = leftDoor.position;
        Vector3 rStart = rightDoor.position;
        Vector3 lTarget = opening ? leftOpenPos : leftClosedPos;
        Vector3 rTarget = opening ? rightOpenPos : rightClosedPos;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * moveSpeed;
            leftDoor.position = Vector3.Lerp(lStart, lTarget, t);
            rightDoor.position = Vector3.Lerp(rStart, rTarget, t);
            yield return null;
        }

        leftDoor.position = lTarget;
        rightDoor.position = rTarget;
    }
}