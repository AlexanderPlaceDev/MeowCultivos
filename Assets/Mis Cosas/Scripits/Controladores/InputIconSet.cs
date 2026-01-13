using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Input/IconosControl")]
public class InputIconSet : ScriptableObject
{
    public Sprite buttonSouth;   // A / X / Cross
    public Sprite buttonNorth;   // Y / Triangle
    public Sprite buttonEast;    // B / Circle
    public Sprite buttonWest;    // X / Square
    public Sprite leftShoulder;
    public Sprite rightShoulder;
    public Sprite leftTrigger;
    public Sprite rightTrigger;
    public Sprite dpadUp;
    public Sprite dpadDown;
    public Sprite dpadLeft;
    public Sprite dpadRight;
    public Sprite leftStick;
    public Sprite rightStick;

    // Opcional: teclado / mouse
    public Sprite leftMouseButton;
    public Sprite rightMouseButton;
    public Sprite middleMouseButton;
}
