using UnityEngine;

public class ButtonTrigger : MonoBehaviour
{
    public BridgeManager bridgeManager;
    public int buttonID; // 1 or 2

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            bridgeManager.ButtonPressed(buttonID);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            bridgeManager.ButtonReleased(buttonID);
    }
}
