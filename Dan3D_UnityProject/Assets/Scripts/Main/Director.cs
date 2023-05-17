using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Director
{
    //Values
    public static float distanceToInteract = 2.0f;
    public static float distanceToInteractRemote = 10.0f;
    public static float displayItemTime = 2.5f;

    //Used for Inventory to know if UI is navigated with keys or mouse
    public enum UI_InputType {
        Mouse,
        Buttons
    };   

    public enum InputDevices {
        Keyboard,
        Controller
    };

    public static UI_InputType inputType = UI_InputType.Buttons; 

    public static InputDevices inputDevice = InputDevices.Keyboard;
}
