using UnityEngine;
using System.Collections;

public class BlockMover : MonoBehaviour, IButtonControlled
{
    [Header("Parent object containing all blocks")]
    public Transform blockParent;

    [Header("Movement Settings")]
    public float moveDistance = 5f;
    public float moveSpeed = 15f;

    private Transform[] blocks;

    private Vector3[] startPositions;
    private Vector3[] downPositions;

    private Coroutine moveRoutine;

    private enum State { AtStart, Down }
    private State currentState = State.AtStart;

    void Start()
    {
        if (blockParent == null)
        {
            Debug.LogError("BlockMover: No block parent assigned!");
            return;
        }

        // Get all children of the parent
        blocks = blockParent.GetComponentsInChildren<Transform>();

        // Remove parent from the array (first element)
        blocks = System.Array.FindAll(blocks, b => b != blockParent);

        if (blocks.Length == 0)
        {
            Debug.LogError("BlockMover: No child blocks found under parent!");
            return;
        }

        startPositions = new Vector3[blocks.Length];
        downPositions  = new Vector3[blocks.Length];

        // Pre-store start and down positions
        for (int i = 0; i < blocks.Length; i++)
        {
            startPositions[i] = blocks[i].position;
            downPositions[i]  = startPositions[i] + Vector3.down * moveDistance;
        }
    }

    // Button 1 → move down (ONLY if currently at start)
    // Button 2 → move up (ONLY if currently down)
    public void ButtonPressed(int buttonID)
    {
        if (buttonID == 1 && currentState == State.AtStart)
        {
            MoveTo(downPositions);
            currentState = State.Down;
        }

        if (buttonID == 2 && currentState == State.Down)
        {
            MoveTo(startPositions);
            currentState = State.AtStart;
        }
    }

    public void ButtonReleased(int buttonID)
    {
        // Do Nothing
    }

    private void MoveTo(Vector3[] targets)
    {
        if (moveRoutine != null)
            StopCoroutine(moveRoutine);

        moveRoutine = StartCoroutine(MoveBlocks(targets));
    }

    private IEnumerator MoveBlocks(Vector3[] targets)
    {
        while (true)
        {
            bool allReached = true;

            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i].position = Vector3.MoveTowards(
                    blocks[i].position,
                    targets[i],
                    moveSpeed * Time.deltaTime
                );

                if (Vector3.Distance(blocks[i].position, targets[i]) > 0.01f)
                    allReached = false;
            }

            if (allReached)
                yield break;

            yield return null;
        }
    }
}