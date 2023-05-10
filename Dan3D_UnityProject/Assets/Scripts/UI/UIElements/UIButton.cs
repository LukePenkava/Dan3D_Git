using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class UIButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IFocusUI
{
    public Image visual;
    public Image focus;
    public Sprite visual_Normal;
    public Sprite visual_Pressed;
    public Color colorNormal;
    public Color colorPressed;
    public Color colorDisabled;
    
    public UnityEvent buttonAction;
    
    public bool disabled = false; 
    public bool disableAutomaticColors = false;

    
    protected virtual void OnEnable() {
        if(!disableAutomaticColors) {
            visual.sprite = visual_Normal;
            visual.color = colorNormal;
        }

        focus.enabled = false;
        UnFocus();
    }

    public void SetDisabled(bool isDisabled) {
        disabled = isDisabled;

        if(disabled) {
            focus.enabled = false;
            visual.color = colorDisabled;            
        }
        else {
            visual.color = colorNormal;
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
      
        if(!disableAutomaticColors) {
            visual.sprite = visual_Pressed;
            visual.color = colorPressed;
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData) {
        if(Director.inputType != Director.UI_InputType.Mouse || disabled) { return; }

        Activate();        
        if(disabled) { return; }

        if(!disableAutomaticColors) {
            visual.sprite = visual_Normal;
            visual.color = colorNormal;
        }
    }

    public void Activate() {
        if(disabled) { return; }

        buttonAction.Invoke();
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
