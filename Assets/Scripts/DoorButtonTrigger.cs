using UnityEngine;

public class DoorButtonTrigger : MonoBehaviour
{
    public DualButtonDoor doorManager;
    public int buttonID = 1; // 1 or 2

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorManager.ButtonPressed(buttonID);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorManager.ButtonReleased(buttonID);
        }
    }
}