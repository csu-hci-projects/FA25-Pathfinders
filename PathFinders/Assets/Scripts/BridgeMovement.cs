using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DualButtonBridge : MonoBehaviour, IButtonControlled
{
    [Header("Assign the parent of the bridge platforms")]
    public Transform platformParent;

    [Header("Movement Settings")]
    public float riseHeight = 19f;         // How far up they move from starting position
    public float moveSpeed = 20f;           // Speed of movement (units per second for MoveTowards)
    public float delayBetweenCubes = 0.15f; // Delay between each cube moving

    // Button states (multi-button ready)
    private bool button1Pressed = false;
    private bool button2Pressed = false;

    // Once true the bridge will stay raised forever
    private bool isPermanentRaise = false;

    private Vector3[] startPositions;
    private Vector3[] endPositions;

    // Track all cube movement coroutines
    private readonly List<Coroutine> activeCubes = new List<Coroutine>();

    private void Start()
    {
        if (platformParent == null)
        {
            Debug.LogError("No platform parent assigned!");
            return;
        }

        int count = platformParent.childCount;
        startPositions = new Vector3[count];
        endPositions = new Vector3[count];

        for (int i = 0; i < count; i++)
        {
            Transform cube = platformParent.GetChild(i);
            startPositions[i] = cube.position;
            endPositions[i] = cube.position + Vector3.up * riseHeight;
        }
    }

    // PUBLIC API - call these from each button's trigger script
    // buttonID should be 1 or 2
    public void ButtonPressed(int buttonID)
    {
        if (buttonID == 1) button1Pressed = true;
        else if (buttonID == 2) button2Pressed = true;
        else Debug.LogWarning("ButtonPressed: unknown id " + buttonID);

        UpdateBridgeState();
    }

    public void ButtonReleased(int buttonID)
    {
        if (buttonID == 1) button1Pressed = false;
        else if (buttonID == 2) button2Pressed = false;
        else Debug.LogWarning("ButtonReleased: unknown id " + buttonID);

        UpdateBridgeState();
    }

    // Decides whether to run normal waves or lock permanently
    private void UpdateBridgeState()
    {
        // If already permanent, nothing changes
        if (isPermanentRaise) return;

        // If both buttons are pressed (simultaneously), lock permanent raise
        if (button1Pressed && button2Pressed)
        {
            isPermanentRaise = true;

            // Immediately stop current cube coroutines and start a permanent raise wave
            foreach (var c in activeCubes)
                if (c != null) StopCoroutine(c);
            activeCubes.Clear();

            // Start the rising wave that ignores button releases
            StartCoroutine(MovePlatformWave(true)); // rising = true
            return;
        }

        // Normal behavior: rise if either pressed, lower if none
        bool shouldRise = (button1Pressed || button2Pressed);

        // Restart movement in the requested direction
        foreach (var c in activeCubes)
            if (c != null) StopCoroutine(c);
        activeCubes.Clear();

        StartCoroutine(MovePlatformWave(shouldRise));
    }

    private IEnumerator MovePlatformWave(bool rising)
    {
        int count = platformParent.childCount;
        int startIndex = rising ? 0 : count - 1;
        int direction = rising ? 1 : -1;

        for (int i = 0; i < count; i++)
        {
            int index = startIndex + i * direction;
            Transform cube = platformParent.GetChild(index);

            // Start moving this cube toward the current target; MoveCubeContinuous checks flags each frame.
            Coroutine c = StartCoroutine(MoveCubeContinuous(cube, index, rising));
            activeCubes.Add(c);

            yield return new WaitForSeconds(delayBetweenCubes);
        }
    }

    private IEnumerator MoveCubeContinuous(Transform cube, int index, bool initialRising)
    {
        // initialRising only used to choose the initial target; after that the coroutine
        // uses the live button flags or permanent flag to decide the target on each frame.
        while (true)
        {
            Vector3 newTarget;

            if (isPermanentRaise)
            {
                newTarget = endPositions[index];
            }
            else
            {
                bool shouldRise = (button1Pressed || button2Pressed);
                newTarget = shouldRise ? endPositions[index] : startPositions[index];
            }

            // Move toward the computed target (units per second)
            cube.position = Vector3.MoveTowards(cube.position, newTarget, moveSpeed * Time.deltaTime);

            // If cube reached its destination, stop this coroutine
            if (Vector3.Distance(cube.position, newTarget) < 0.01f)
                yield break;

            yield return null;
        }
    }
    public void SetPermanentState(bool up)
    {
        // Stop all cube movement
        foreach (var c in activeCubes)
            if (c != null) StopCoroutine(c);
        activeCubes.Clear();

        // Instantly move all cubes to the target state
        for (int i = 0; i < platformParent.childCount; i++)
        {
            Transform cube = platformParent.GetChild(i);
            cube.position = up ? endPositions[i] : startPositions[i];
        }
    }
}