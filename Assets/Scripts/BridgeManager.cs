using UnityEngine;

public class BridgeManager : MonoBehaviour
{
    [Header("Reference to the bridge controller")]
    public PressureButtonBridge bridge;

    private bool button1Pressed;
    private bool button2Pressed;

    public void ButtonPressed(int id)
    {
        if (id == 1) button1Pressed = true;
        else if (id == 2) button2Pressed = true;

        bridge.ButtonPressed(id);
    }

    public void ButtonReleased(int id)
    {
        if (id == 1) button1Pressed = false;
        else if (id == 2) button2Pressed = false;

        bridge.ButtonReleased(id);
    }
}
