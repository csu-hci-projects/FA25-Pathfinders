using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class L3MovePlatform : MonoBehaviour, IButtonControlled
{
    [Header("Assign the Platform")]
    public Transform platform;

    [Header("Movement Settings")]
    public float moveSpeed = 10f;

    // Button states
    private bool button1Pressed = false;
    private bool button2Pressed = false;

    // Waypoints for the multi-step path
    private List<Vector3> waypoints = new List<Vector3>();
    private Coroutine moveRoutine;
    private Vector3 currentTarget;
    private bool isMoving = false;

    void Start()
    {
        if (platform == null)
        {
            Debug.LogError("No platform assigned!");
            return;
        }

        Vector3 start = platform.position;

        // Build waypoint path
        waypoints.Add(start);
        waypoints.Add(start + new Vector3(0, 0, 6));     // +6 Z
        waypoints.Add(start + new Vector3(0, -15, 6));   // -15 Y
        waypoints.Add(start + new Vector3(0, -15, 21));  // +15 Z
        waypoints.Add(start + new Vector3(0, -10, 21));  // +5 Y
        waypoints.Add(start + new Vector3(0, -10, 71));  // +50 Z
    }

    public void ButtonPressed(int buttonID)
    {
        if (buttonID == 1) button1Pressed = true;
        else if (buttonID == 2) button2Pressed = true;

        UpdatePlatformState();
    }

    public void ButtonReleased(int buttonID)
    {
        if (buttonID == 1) button1Pressed = false;
        else if (buttonID == 2) button2Pressed = false;

        UpdatePlatformState();
    }

    private void UpdatePlatformState()
    {
        bool shouldFollowPath = (button1Pressed || button2Pressed);

        if (moveRoutine != null){
            StopCoroutine(moveRoutine);
        }

        moveRoutine = StartCoroutine(FollowPath(shouldFollowPath));
    }

    private IEnumerator FollowPath(bool moving)
    {
        if (waypoints.Count < 2) {
            yield break;
        }

        if (moving)
        {
            // Move from waypoint 0 → last
            for (int i = 1; i < waypoints.Count; i++)
            {
                yield return MoveToWaypoint(i);
                if (!button1Pressed && !button2Pressed)
                    yield break; // stop early if buttons released
            }
        }
        else
        {
            // Move from waypoint last → 0
            for (int i = waypoints.Count - 2; i >= 0; i--)
            {
                yield return MoveToWaypoint(i);
                if (button1Pressed || button2Pressed)
                    yield break; // stop early if buttons pressed
            }
        }
    }

    private IEnumerator MoveToWaypoint(int waypointIndex)
    {
        currentTarget = waypoints[waypointIndex];
        isMoving = true;

        while (Vector3.Distance(platform.position, currentTarget) > 0.01f) {
            yield return new WaitForFixedUpdate();
        }

        isMoving = false;
    }

    private void FixedUpdate()
    {
        if (isMoving) {
            platform.position = Vector3.MoveTowards(platform.position, currentTarget, moveSpeed * Time.fixedDeltaTime);
        }
    }
}