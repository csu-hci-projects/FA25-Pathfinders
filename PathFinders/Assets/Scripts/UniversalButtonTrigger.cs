using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ButtonTrigger : MonoBehaviour
{
    [Header("Button Settings")]
    public MonoBehaviour targetObject; // Assign your bridge/door/elevator manager here
    public int buttonID = 1;            // 1 or 2
   

    private IButtonControlled controlledTarget;

    private void Awake()
    {
        // Try to find the IButtonControlled interface on the assigned script
        if (targetObject != null)
        {
            controlledTarget = targetObject as IButtonControlled;

            if (controlledTarget == null)
                Debug.LogError($"{targetObject.name} does not implement IButtonControlled!");
        }
        else
        {
            Debug.LogError($"{name} has no targetObject assigned!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (controlledTarget != null && (other.CompareTag("Player1") || other.CompareTag("Player2")))
        {
            controlledTarget.ButtonPressed(buttonID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (controlledTarget != null && (other.CompareTag("Player1") || other.CompareTag("Player2")))
        {
            controlledTarget.ButtonReleased(buttonID);
        }
    }
}