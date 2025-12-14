using UnityEngine;
using System.Collections;

public class DualButtonElevator : MonoBehaviour, IButtonControlled
{
    [Header("Assign the Elevator")]
    public Transform elevator;

    [Header("Movement Settings")]
    public float liftHeight = 10f;         // How far up they move from starting position
    public float moveSpeed = 3f;          // Speed of movement (units per second for MoveTowards)

    // Button states (multi-button ready)
    private bool button1Pressed = false;
    private bool button2Pressed = false;

    private Vector3 startPosition;
    private Vector3 endPosition;

    private Coroutine moveRoutine;

    private Rigidbody rb; //added

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (elevator == null)
        {
            Debug.LogError("No elevator assigned!");
            return;
        }

        rb = GetComponent<Rigidbody>(); //added
        startPosition = elevator.position; ;
        endPosition = startPosition + Vector3.up * liftHeight;
    }

    public void ButtonPressed(int buttonID)
    {
        if (buttonID == 1) button1Pressed = true;
        else if (buttonID == 2) button2Pressed = true;
        else Debug.LogWarning("Unknown Button ID: " + buttonID);

        UpdateElevatorState();
    }


    public void ButtonReleased(int buttonID)
    {
        if (buttonID == 1) button1Pressed = false;
        else if (buttonID == 2) button2Pressed = false;
        else Debug.LogWarning("Unknown Button ID: " + buttonID);

        UpdateElevatorState();
    }

    private void UpdateElevatorState()
    {
        bool shouldRise = (button1Pressed || button2Pressed);

        // Stop current movement before changing direction
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveElevator(shouldRise));
    }

    private IEnumerator MoveElevator(bool rising)
    {
        Vector3 target = rising ? endPosition : startPosition;

        while (true)
        {
            // Allow direction to change mid-motion
            bool shouldRise = (button1Pressed || button2Pressed);
            Vector3 newTarget = shouldRise ? endPosition : startPosition;

            //elevator.position = Vector3.MoveTowards(elevator.position, newTarget, moveSpeed * Time.deltaTime);
            rb.MovePosition(Vector3.MoveTowards(rb.position, newTarget, moveSpeed * Time.fixedDeltaTime)); //added

            if (Vector3.Distance(elevator.position, newTarget) < 0.01f)
                yield break;

            yield return null;
        }
    }
}
