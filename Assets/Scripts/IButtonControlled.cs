using UnityEngine;

public interface IButtonControlled
{
    void ButtonPressed(int buttonID);
    void ButtonReleased(int buttonID);
}