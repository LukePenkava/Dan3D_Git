using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIButtonSwitch : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IFocusUI
{
    int buttonIndex = -1; //Set Index in inspector in Action's paramter, this is just var for the param

    public Image visual;
    public Image focus;
    //public Sprite visual_Normal;
    //public Sprite visual_Pressed;
    public Color colorNotSelected;
    public Color colorSelected;
    public Color colorDisabled;
    
    public UnityEvent<int> buttonAction;
    public bool disabled = false; 

    
    protected virtual void OnEnable() {  
        UnFocus();
    }

    public void SetVisual(bool isSelected) {
        visual.color = isSelected ? colorSelected : colorNotSelected;
    }

    public void SetDisabled(bool isDisabled) {
        disabled = isDisabled;

        if(disabled) {
            focus.enabled = false;
            visual.color = colorDisabled;            
        }
        else {
            visual.color = colorNotSelected;
        }       
    }
   
    public void OnPointerEnter(PointerEventData pointerEventData) {
        if(Director.inputType != Director.UI_InputType.Mouse || disabled) { return; }

        SetFocus();
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        if(Director.inputType != Director.UI_InputType.Mouse || disabled) { return; }
        
        UnFocus();
    }

    public void OnPointerDown(PointerEventData pointerEventData){
        if(Director.inputType != Director.UI_InputType.Mouse || disabled) { return; }          
    }

    public void OnPointerUp(PointerEventData pointerEventData) {
        if(Director.inputType != Director.UI_InputType.Mouse || disabled) { return; }

        Activate();               
    }

    public void Activate() {
        if(disabled) { return; }

        buttonAction.Invoke(buttonIndex);
    }

    public void SetFocus() {
        if(disabled) { return; }

        focus.enabled = true;      
    }

    public void UnFocus() {
        if(disabled) { return; }

        focus.enabled = false;
    }

    public bool CanBeFocused() {
        return !disabled;
    }

    public Vector2 GetPosition() {
        return new Vector2(this.transform.position.x, this.transform.position.y);
    }

    public Vector2 GetLocalPosition() {
        return new Vector2(this.transform.localPosition.x, this.transform.localPosition.y);
    }

    public UIScrollArea GetScrollArea() {
        return null;
    }
    
}
