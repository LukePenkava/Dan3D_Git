using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IFocusUI
{
    Character_BaseData data;
    Inventory inventoryScript;
    UIManager uiManager;
    UIScrollArea scrollArea;

    public Image visual;
    public Image focus;
    public GameObject itemParent;

    int slotIndex = 0;
    public int SlotIndex {
        get { return slotIndex; }
    }

    bool isOccupied = false;
    public bool IsOccupied {
        get { return isOccupied; }
        set { isOccupied = value; }
    }

    int itemIndex = -1;  //Index of item in the inventoryObjects in Inventory. Slot does not hold actual item, just reference to the index of the item
    public int ItemIndex {
        get { return itemIndex; }
    }

    public void Init(int _slotIndex, float size, Inventory script, UIManager uiManager_Script, UIScrollArea ScrollArea, Character_BaseData Data) {
        
        data = Data;
        inventoryScript = script;
        uiManager = uiManager_Script;
        scrollArea = ScrollArea;

        slotIndex = _slotIndex;
        visual.rectTransform.sizeDelta = new Vector2(size, size);
        focus.rectTransform.sizeDelta = new Vector2(size, size);
        focus.enabled = false;
    }

    public void SetItem(int _itemIndex) {
        isOccupied = true;
        itemIndex = _itemIndex;
    }

    public void RemoveItem() {
        itemIndex = -1;
        isOccupied = false;
    }

    public void OnPointerClick(PointerEventData pointerEventData) {
        if(Director.inputType != Director.UI_InputType.Mouse) { return; }

        Activate();
    }

    public void OnPointerEnter(PointerEventData pointerEventData) {
        if(Director.inputType != Director.UI_InputType.Mouse) { return; }

        if(isOccupied == false) {
            SetFocus();
        }     
    }

    public void OnPointerExit(PointerEventData pointerEventData) {
       if(Director.inputType != Director.UI_InputType.Mouse) { return; }

        UnFocus();     
    }

    //Move item to this slot
    public void Activate() {
        
        //Move the item to this slot, if there is some item selected and this slot is free
        if(inventoryScript.SelectedSlotIndex >= 0 && isOccupied == false && inventoryScript.ItemsMovable) {
           
            //SelectedItemIndex is a slot index, thats how slots and inventory objects are organized in their dictionaries and accessed, by slot index
            int selectedItemIndex = inventoryScript.SelectedSlotIndex;
            inventoryScript.UnselectItem();
            inventoryScript.Slots[selectedItemIndex].RemoveItem();

            //Saved Items are not organized by slot index, its just list, so it can be easily converted to Saving. Find index of selected item in the savedItems by the index
            int savedItemIndex = data.GetItemListIndexFromSlotIndex(selectedItemIndex);
            //To access actual game object, we need the current slotindex of this item, while this new slot is still empty
            int previousSlotIndex = data.ItemsList[savedItemIndex].slotIndex;          
           
            
            //Adjust and Move Item GameObject ( InventoryObject )
            //Get Access to selected object, while it still sits in previous slot
            InventoryObject invObj = inventoryScript.inventoryObjects[previousSlotIndex];
            //Change objects parent/position to this slot
            invObj.gameObject.transform.parent = itemParent.transform;
            invObj.gameObject.transform.localPosition = Vector3.zero;
            //Adjust its slot data to new one
            invObj.SlotIndex = slotIndex;
            //Remove the object at old slot position
            inventoryScript.inventoryObjects.Remove(previousSlotIndex);      
            //Add the same object at new position    
            inventoryScript.inventoryObjects.Add(slotIndex, invObj);

            data.UpdateItem(savedItemIndex, slotIndex);
            isOccupied = true;  

            //invObj.Activate();
            invObj.SetFocus();
            
            if(Director.inputType == Director.UI_InputType.Buttons) {
                uiManager.ItemMoved(ref invObj);
            }
        }
    }

    public void SetFocus() {
        focus.enabled = true;
    }

    public void UnFocus() {
        focus.enabled = false;
    }

    public bool CanBeFocused() {
        return !isOccupied;
    }

    public Vector2 GetPosition() {
        return new Vector2(this.transform.position.x, this.transform.position.y);
    }

    public Vector2 GetLocalPosition() {
        return new Vector2(this.transform.localPosition.x, this.transform.localPosition.y);
    }

    public UIScrollArea GetScrollArea() {
        return scrollArea;
    }
}

