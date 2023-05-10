using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSelectionObject : MonoBehaviour
{
    UIManager uiManager;

    public Text interactionText;
    public Image buttonImage;
    public Text buttonText; 

    Interaction activeInteraction;
    int selectionIndex = 0;
    InteractionStates lastState = InteractionStates.Null;

    Color colorTextActive = new Color(111F/255f, 89F/255f, 76F/255f, 1.0f);
    Color colorTextDisabled = new Color(111F/255f, 89F/255f, 76F/255f, 0.4f);

    string[] keyboardInputs = new string[]{"E", "R", "T"};
    string[] joystickInputs = new string[]{"X", "Y", "B"};

    ButtonColors[] keyboardButtons = new ButtonColors[]{ButtonColors.Green, ButtonColors.Purple, ButtonColors.Red};
    ButtonColors[] joystickButtons = new ButtonColors[]{ButtonColors.Green, ButtonColors.Purple, ButtonColors.Red};


    void Update() {

        if(lastState != activeInteraction.InteractionState) {
           
            Set();
            lastState = activeInteraction.InteractionState;
        }
    }

    public void Setup(Interaction interaction, int SelectionIndex) {

        uiManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<UIManager>();
        activeInteraction = interaction;
        selectionIndex = SelectionIndex;
        lastState = interaction.InteractionState;
        Set();
    }

    void Set() {      

        bool isActive = activeInteraction.InteractionState == InteractionStates.Active;

         if(activeInteraction.customDisplayText.Length > 0) {
            activeInteraction.Data.name = activeInteraction.customDisplayText;
        }

        interactionText.text = activeInteraction.Data.name.ToUpper();
        interactionText.color = isActive ? colorTextActive : colorTextDisabled;

        ButtonVisual buttonVisual = new ButtonVisual();
        if(isActive) {            
            ButtonColors buttonColor = keyboardButtons[selectionIndex]; //(Director.inputDevice == Director.InputDevice.Keyboard) ? keyboardButtons[selectionIndex] : joystickButtons[selectionIndex];
            buttonVisual = uiManager.Buttons[buttonColor];
        } else {
            buttonVisual = uiManager.Buttons[ButtonColors.Grey];
        }

        buttonImage.sprite = buttonVisual.buttonSprite;
        buttonImage.color = buttonVisual.buttonColor;        

        buttonText.text = keyboardInputs[selectionIndex]; //(Director.inputDevice == Director.InputDevice.Keyboard) ? keyboardInputs[selectionIndex] : joystickInputs[selectionIndex];
        buttonText.color = buttonVisual.buttonTextColor;

       LayoutRebuilder.ForceRebuildLayoutImmediate(interactionText.gameObject.GetComponent<RectTransform>());
    }
}




