using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

//Visual Representation of InventoryItem. This is a gameObject player can see and interact with
public class InventoryObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IFocusUI
{
    Inventory inventoryScript;
    UIScrollArea scrollArea;

    public Image iconVisual;
    public Image focus;
    public Image selectedVisual;

    Color normalColor = Color.white;
    Color tradingColor = Color.grey;

    Item itemData;
    public Item ItemData {
        get { return itemData; }
    }
    
    int slotIndex = -1;
    public int SlotIndex {
        get { return slotIndex; }
        set { slotIndex = value; }
    }

    //Item was selected for trade and now is in the active trading window considered for trade
    bool trading = false;
    public bool Trading {
        get { return trading; }
    }

    float lastClickTime = 0f;
    float registerDoubleClickTime = 0.25f; 

    //Inventory Object can have double click action. First one is always activate and second one is optional any function call passed as an action.
    //For Trade its for example Add or Remove from trade
    Action doubleClickAction;


    public void Init(Item ItemData, float size, int SlotIndex, Inventory script, UIScrollArea ScrollArea, Action DoubleClickAction = null) {
        inventoryScript = script;
        scrollArea = ScrollArea;
        doubleClickAction = DoubleClickAction;

        itemData = ItemData;
        slotIndex = SlotIndex;

        iconVisual.sprite = Resources.Load<Sprite>("Icons/Items/" + itemData.itemName.ToString());
        iconVisual.rectTransform.sizeDelta = new Vector2(size, size);
        focus.rectTransform.sizeDelta = new Vector2(size, size);
        selectedVisual.rectTransform.sizeDelta = new Vector2(size, size);
        focus.enabled = false;
        selectedVisual.enabled = false;        
    }

    public void SetTrading(bool setTrading) {
        trading = setTrading;
        iconVisual.color = trading ? tradingColor : normalColor;
    }


    public void OnPointerEnter(PointerEventData pointerEventData) {
        if(Director.inputType != Director.UI_InputType.Mouse) { return; }

        SetFocus();
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
        if(Director.inputType != Director.UI_InputType.Mouse) { return; }

        UnFocus();
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        if(Director.inputType != Director.UI_InputType.Mouse) { return; }

        float currentClickTime = Time.time;
        float delta = currentClickTime - lastClickTime;
        lastClickTime = Time.time;

        //Registering double click means also registering normal click first in the same action
        if(delta < registerDoubleClickTime) {

            if(doubleClickAction != null) {   
                //If item is not selected, select to be able to process double click action. When player selects item with one click and then double clicks, he firest deselects item and then doubleclick is getting registered => problem
                if(inventoryScript.SelectedSlotIndex < 0) { Activate(); }           
                doubleClickAction();
                lastClickTime = 0f; //Prevent 3 clicks causing two times doubleClick
            }
        } 
        else {
            //This gets always registered, even at double click, this is the first click
            Activate();
        }           
    }

    public void Activate() {
        inventoryScript.SelectItem(slotIndex);
    }

    public void SetSelectedVisual(bool isSelected) {
        selectedVisual.enabled = isSelected;
    }

    public void SetFocus() {
        focus.enabled = true;
    }

    public void UnFocus() {
        focus.enabled = false;
    }

    public bool CanBeFocused() {
        return true;
    }

    public Vector2 GetPosition() {
        return new Vector2(this.transform.position.x, this.transform.position.y);
    }

    public Vector2 GetLocalPosition() {
        Vector2 slotLocalPos = inventoryScript.Slots[slotIndex].gameObject.transform.localPosition;
        return new Vector2(slotLocalPos.x, slotLocalPos.y);
    }

    public UIScrollArea GetScrollArea() {
        return scrollArea;
    }
}

