using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UISwitch : MonoBehaviour
{
    //Switch is parent which holds all switch buttons
    //All the switch buttons call Switch function here, sending parameter what index that button was
    //Switch then calls actual final function with index of the button
    //Switch sets visuals for all switch buttons ( selected button etc )

    public List<UIButtonSwitch> buttons;
    public UnityEvent<int> buttonAction;

    // Start is called before the first frame update
    void Awake()
    {
        foreach(Transform child in this.transform) {
            buttons.Add(child.gameObject.GetComponent<UIButtonSwitch>());
        }
    }

    //This gets called by all buttons, if there are only 2 or 5 buttons
    public void Switch(int index) {
        foreach(UIButtonSwitch button in buttons) {
            button.SetVisual(false);
        }
        buttons[index].SetVisual(true);

        buttonAction.Invoke(index);
    }
}
