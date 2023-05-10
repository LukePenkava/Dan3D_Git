using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;


public class Inventory : MonoBehaviour
{
    #region Setup    

    public delegate void GiveItemDelegate(Characters.Names charName, Items.ItemName itemName, Items.ItemTags[] itemTags);
    public static event GiveItemDelegate GiveItemEvent;

    public enum InventoryType {
        MainInventory,
        Trade,
        GiveItem
    }
    InventoryType activeType;

    public UIManager uiManager;    
    public InventoryItemDisplay itemDisplay;    
    Character_BaseData data;  

    public UIScrollArea itemsScrollArea;

    public delegate void ItemSelected(Item itemData);
    public event ItemSelected OnItemSelected;

    public delegate void ItemUnSelected();
    public event ItemUnSelected OnItemUnSelected;

    bool itemsMovable = true;
    public bool ItemsMovable {
        get { return itemsMovable; }
    }

    Action doubleClickAction;

    #endregion

    #region Slots

    //Slots List index is essentially same as actual slot number/position, ie slots[0] is always first etc. Create in for loop from 0
    List<InventorySlot> slots = new List<InventorySlot>();    
    public List<InventorySlot> Slots {
        get { return slots; }
    }

    public Transform slotsParent;
    public GameObject slotPrefab;

    //public int slotsAmount = 10;
    public int slotsInRow = 5; //How many slots in row next to each other ( on one line )
    float gapBetweenItems = 13;  //25 27
    float slotSize = 0;
    float step = 0;
    float itemPadding = 0f;
    Vector2 scrollAreaSize = Vector2.zero;

    #endregion

    #region Items   

    //Key is slotIndex, ie where this object sits in inventory
    //Actual Item Objects in Inventory (Icons, GameObjects)
    public Dictionary<int, InventoryObject> inventoryObjects = new Dictionary<int, InventoryObject>(); 

    public GameObject inventoryObjectPrefab;
    public GameObject buttonUse;
    public GameObject buttonRead;
    public GameObject buttonDrop;
    public GameObject buttonGive;

    int selectedSlotIndex = -1;
    public int SelectedSlotIndex {
        get { return selectedSlotIndex; }
    }

    //GiveItem  
    public Characters.Names charNameGiveItem;
    Items.ItemName giveItemName;
    Items.ItemTags[] giveItemTags;

    #endregion     



    #region Setup  

    //Double Click for Inventory Item to call any other function after it has been activated to improve user experience
    public void Open(Character_BaseData Data, InventoryType type, Action DoubleClickAction = null, Items.ItemTags itemsFilter = Items.ItemTags.Neutral) {
        data = Data;
        itemsMovable = data.isPlayer;
        doubleClickAction = DoubleClickAction;
        activeType = type;                

        if(activeType == InventoryType.MainInventory || activeType == InventoryType.GiveItem) {
            buttonUse.SetActive(false);
            buttonDrop.SetActive(false);
            buttonGive.SetActive(false);
            buttonRead.SetActive(false);
        }

        DestroySlots();
        SetItemsScrollArea();          
        CreateSlots();        
        CreateItems(itemsFilter);               
    } 

    public void Close() {
         DestroySlots(); 
         data = null;
    }

    void SetItemsScrollArea() {        

        float scrollAreaPadding = itemPadding;         

        //Set gaps around scroll area so the there is same gap around scroll as gaps between items inside scroll area. use the size to init the scroll area     
        scrollAreaSize = itemsScrollArea.SetGaps(gapBetweenItems);             
        //Amount of space for each item 
        step = (scrollAreaSize.x / slotsInRow);  
        //How many slots there is vertically based on slots amount
        int slotsInColumn = Mathf.CeilToInt((float)data.SlotsAmount / (float)slotsInRow); 
        //TopLimit for scrollArea scroling, difference between the whole size need for all items vertically and actual size of scrollArea
        float totalLength = (slotsInColumn * step) - scrollAreaSize.y + scrollAreaPadding;             
        //Init ScrollArea
        itemsScrollArea.Init(totalLength, 0f, step, scrollAreaSize.y, gapBetweenItems);

    }

    #endregion


    #region Slots
    
    //Create Slots, where items can slotted into. Slots are not changing, are stable, items can move around in slots. 
    void CreateSlots() {       

        float startPosX = (scrollAreaSize.x / -2f) + (step / 2f);  //Start position start at left side. That is half of the area minues to the left ( 800 width, start at -400), first pos starts at the center for step,, it put icon into the center of the space for the icon
        slotSize = step - gapBetweenItems; //Make the icons smaller, so there is room for gap withing the size for each icon. Half gap on left and right for each item, both items next to each other add full gap
   
        float startPosY =  (slotSize / -2f) - (gapBetweenItems); 
        int row = 0;
        int xIndex = 0;

        for(int i = 0; i < data.SlotsAmount; i++) {
            
            row = Mathf.FloorToInt(i / slotsInRow);
            xIndex = i % slotsInRow;

            //Create Slot
            GameObject newSlot = Instantiate(slotPrefab, Vector3.zero, Quaternion.identity);
            newSlot.transform.SetParent(slotsParent, false);
            newSlot.transform.localPosition = new Vector3(startPosX + (xIndex * step), startPosY - (row * step), 0);
            newSlot.name = "Slot" + i.ToString();
            InventorySlot newInvSlot = newSlot.GetComponent<InventorySlot>();
            newInvSlot.Init(i, slotSize, this, uiManager, itemsScrollArea, data);
               
            slots.Add(newInvSlot);
        }
    }

    //Destroy Slot GameObjects and items in them
    void DestroySlots() {
        foreach(InventorySlot slot in slots) {
            Destroy(slot.gameObject);
        }

        slots.Clear();
        //InventoryObjects are children of slots, so they get deleted with slot
        inventoryObjects.Clear();
    }

    #endregion

    
    #region Items 

    void CreateItems(Items.ItemTags filterTag = Items.ItemTags.Neutral) {
        float itemSize = slotSize - itemPadding; //padding between item and lost          
        
        foreach(InventoryItem item in data.ItemsList) {

            //Filtering items, if filterTag is not neutral, there is filtering
            //If item does not have this tag, continue and skip this item, dont show it inventory
            if(filterTag != Items.ItemTags.Neutral) {
                if(ItemsDirectory.ItemsData[item.itemName].CheckTag(filterTag) == false) {
                    continue;
                }
            }

            int slotIndex = item.slotIndex;

            GameObject newGO = Instantiate(inventoryObjectPrefab, Vector3.zero, Quaternion.identity);
            newGO.transform.SetParent(slots[slotIndex].itemParent.transform, false); //Assign the item under correct slot
            newGO.transform.localPosition = Vector3.zero;
            newGO.name = "Item" + slotIndex.ToString();

            InventoryObject invObj = newGO.GetComponent<InventoryObject>();            
            invObj.Init(ItemsDirectory.ItemsData[item.itemName], itemSize, slotIndex, this, itemsScrollArea, doubleClickAction); //Creating actual GameObject for this item
            inventoryObjects.Add(slotIndex, invObj);    

            //Let Slot know what item is there
            slots[slotIndex].SetItem(slotIndex);        

            //Swap position of empty slow with occupied slot, if occupied slot has some empty slot in front of it
            if(filterTag != Items.ItemTags.Neutral) {
                for(int i = 0; i < slots.Count; i++) {
                    if(i < slotIndex) {
                        if(slots[i].IsOccupied == false) {
                            Vector3 emptySlotPos = slots[i].gameObject.transform.localPosition;
                            Vector3 curSlotPos = slots[slotIndex].gameObject.transform.localPosition;
                            InventorySlot newSlot =  slots[slotIndex];
                            InventorySlot emptySlot = slots[i];
                            slots[i].gameObject.transform.localPosition = curSlotPos;
                            slots[slotIndex].gameObject.transform.localPosition = emptySlotPos;
                            
                            //Swap positions of slots in the list. This way the empty slot can be seen by next items in correct spot
                            slots[i] = newSlot;
                            slots[slotIndex] = emptySlot;
                            break;
                        }
                    }
                }    
            }            
        }

        if(filterTag != Items.ItemTags.Neutral) {
            //Hide empty slots
            for(int i = 0; i < slots.Count; i++) {                
                if(slots[i].IsOccupied == false) {
                    slots[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void DropItem() {
        if(selectedSlotIndex < 0) { return; }

        //Remove item from slot, free up the Slot ( selectedItem is already slotIndex, just use that)
        slots[selectedSlotIndex].RemoveItem(); 
        //Destroy actual GameObject of the item 
        Destroy(inventoryObjects[selectedSlotIndex].gameObject);
        //Player does not have this item anymore, remove it. Later adjust it in SaveManager
   
        List<int> slotIndexes = new List<int>(){ selectedSlotIndex };    
        data.RemoveItemsFromSlots(slotIndexes);
        //Remove it from List of inventory items
        inventoryObjects.Remove(selectedSlotIndex);
        UnselectItem();
    }

    public void UseItem() {
        //Apply whatever Item does

        // foreach(KeyValuePair<string, string> useFunction in inventoryObjects[selectedSlotIndex].ItemData.useFunctions) {
        //     if(useFunction.Key == "ChangeStatus") {
        //         if(useFunction.Value == "Curse") {
        //             float value = float.Parse(inventoryObjects[selectedSlotIndex].ItemData.useFunctions["Value"], CultureInfo.InvariantCulture);                   
        //             data.gameObject.GetComponent<CharacterStatus>().CurseMultiplier = value;
        //         }
        //     }

        //     if(useFunction.Key == "StaminaRecovery") {
        //         if(useFunction.Value == "Faster") {
        //             float value = float.Parse(inventoryObjects[selectedSlotIndex].ItemData.useFunctions["Value"], CultureInfo.InvariantCulture); 
        //             if(data.Name == Characters.Names.Dangoru) {
        //                 GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().SetStaminaRecoveryFaster(value);
        //             }                 
        //         }
        //     }
        // }

        //DropItem();
    }

    
    //Set Item as Neutral, if its not supposed to be specific item
    public void GiveItemParams(Items.ItemName giveName, Items.ItemTags[] giveTags) {        
        giveItemName = giveName;     
        giveItemTags = giveTags;        
    }

    public void GiveItem() {
        InventoryObject itemToGive = inventoryObjects[selectedSlotIndex];
        DropItem();

        //First Save progress in quest manager
        GameObject.FindGameObjectWithTag("Managers").GetComponent<QuestManager>().GiveItemEvent(charNameGiveItem, itemToGive.ItemData.itemName, itemToGive.ItemData.tags);

        if(GiveItemEvent != null) {
            GiveItemEvent(charNameGiveItem, itemToGive.ItemData.itemName, itemToGive.ItemData.tags);
        }
        
        //trigger dialogue with the character. 
    }

    // public void ReadRecipe() {
    //     GameData gameData = SaveManager.LoadGameData(); 
    //     foreach(var recipe in inventoryObjects[selectedSlotIndex].ItemData.useFunctions) {                          
    //         gameData.unlockedRecipes[recipe.Value] = true;    
    //         uiManager.ShowNarrator(recipe.Value + " recipe was added.", 1.5f);         
    //     }    
    //     SaveManager.SaveGameData(gameData);    
    //     DropItem();        
    // }
    
    //Called from InventoryObject itself
    public void SelectItem(int slotIndex) { 

        //First hide all buttons, show them later for specific items etc
        if(activeType == InventoryType.MainInventory || activeType == InventoryType.GiveItem) {
            buttonDrop.SetActive(false);
            buttonUse.SetActive(false); 
            buttonRead.SetActive(false);       
        }

        //Clicked/selected same item, which is already selected, unselect it
        if(slotIndex == selectedSlotIndex) {
                UnselectItem();
                return;
        }

        //Selecting new item, deselect previous one
        if(selectedSlotIndex >= 0) {
            if(inventoryObjects.ContainsKey(selectedSlotIndex)) {
                inventoryObjects[selectedSlotIndex].SetSelectedVisual(false); 
            }
        }

        selectedSlotIndex = slotIndex;
        inventoryObjects[slotIndex].SetSelectedVisual(true);

        itemDisplay.SetItemInfo(true, inventoryObjects[slotIndex].ItemData);

        if(OnItemSelected != null) {
            OnItemSelected(inventoryObjects[slotIndex].ItemData);
        }

        if(activeType == Inventory.InventoryType.MainInventory) {
            if(inventoryObjects[slotIndex].ItemData.isUsable) {
                if(inventoryObjects[slotIndex].ItemData.CheckTag(Items.ItemTags.Recipe)) {
                    buttonRead.SetActive(true);                   
                } else {
                    buttonUse.SetActive(inventoryObjects[slotIndex].ItemData.isUsable);
                    buttonDrop.SetActive(true);
                }
            }           
        }

        // if(activeType == InventoryType.GiveItem) {            
        //     bool showGiveItem = false;

        //     if(inventoryObjects[slotIndex].ItemData.itemName == giveItemName) {
        //         showGiveItem = true;
        //     }            
            
        //     //Look through all required tags for give item and all tags selected item has, if there is any match. If so, item can be given      
        //     if(giveItemTags.Length > 0) {     
        //         foreach(Items.ItemTags giveTag in giveItemTags) {                
        //             foreach(Items.ItemTags selectItemTag in inventoryObjects[slotIndex].ItemData.tags) {
        //                 if(selectItemTag == giveTag) {                       
        //                     showGiveItem = true;;
        //                 }
        //             }
        //         } 
        //     }

        //     buttonGive.SetActive(showGiveItem);          
        // }
    }

    public void UnselectItem() {
        if(selectedSlotIndex >= 0) {
            if(inventoryObjects.ContainsKey(selectedSlotIndex)) {
                inventoryObjects[selectedSlotIndex].SetSelectedVisual(false);
            }
        }

        selectedSlotIndex = -1;        
        itemDisplay.SetItemInfo(false);

        if(OnItemUnSelected != null) {
            OnItemUnSelected();
        }

        if(activeType == Inventory.InventoryType.MainInventory || activeType == InventoryType.GiveItem) {
            buttonDrop.SetActive(false);
            buttonUse.SetActive(false); 
            buttonRead.SetActive(false); 
        }
    }

   

    #endregion
  
}



