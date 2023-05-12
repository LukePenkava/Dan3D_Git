using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Holds data like what characther this is, list of theirs items etc
public class Character_BaseData : MonoBehaviour
{
    public Characters.Names Name;
    public GameDirector.DayPhase characterDayPhase = GameDirector.DayPhase.Day;
    public bool isPlayer = false;
    public bool canTrade = false;

    int slotsAmount = 36; //Should be multiple of slotsInRow 
    public int SlotsAmount {
        get { return slotsAmount; }
    }

    //List of items this character has
    List<InventoryItem> itemsList = new List<InventoryItem>();
    public List<InventoryItem> ItemsList {
        get { return itemsList; }
        set { itemsList = value; }
    }

    Dictionary<Items.ItemName, int> tradeValues = new Dictionary<Items.ItemName, int>();
    public Dictionary<Items.ItemName, int> TradeValues {
        get { return tradeValues; }
    }

    //Send to inventory base data and use only these items as source of truth. This now does not require access to inventory to manage characters items
    //Set amount of slots in inventory from here
    //send by refernce
    void Awake() {
        
        if(!isPlayer)
            Init();

        //SetHiddenState(GameDirector.dayPhase);
    }

    public void Init() {

        if(isPlayer) {
            LoadItems();
        } 
        else {

            //Character has items and can trade
            // if(canTrade) {
            //     //Requires CharacterItems Script
            //     CharacterItems itemsScript = GetComponent<CharacterItems>();

            //     if(itemsScript != null) {
            //         itemsScript.Init();
            //     } 
            //     else {
            //         Debug.Log("Missing CharacterItems Script !!!");
            //     }
            // }
        }

        // GetComponent<SpriteAnimator>().Init();
        // GetComponent<Character2D>().Init();
        // GetComponent<CharacterPhysics>().Init();
    }

    // void OnEnable() {
    //     TimeManager.DayPhaseChanged += DayPhaseChanged;
    // }

    // void OnDisable() {
    //     TimeManager.DayPhaseChanged -= DayPhaseChanged;
    // }
    
    //Called by CharacterItems, if character does not have that script, there is not point in loading items anyway
    public void LoadItems() {        
        itemsList = SaveManager.LoadItems(Name);
        //print("itemsList " + itemsList.Count);
    }

    public void SetTradeValues(Dictionary<Items.ItemName, int> values) {
        tradeValues = values;
    }

     
    public void AddItems(List<Items.ItemName> itemNames, bool displayForPlayer = false) {

        //Get all occupied slots from items player has. store them in list
        List<int> occupiedSlots = new List<int>();

        foreach(InventoryItem tempItem in itemsList) {
                occupiedSlots.Add(tempItem.slotIndex);
        }

        //Go through all avaialbe slots and check if the list contains this slot, if it does not, it means no item is using this slot and slot is then free. Use it
        for(int x = 0; x < itemNames.Count; x++) {
            for(int i = 0; i < slotsAmount; i++) {
                if(occupiedSlots.Contains(i) == false) {
                    //Found Empty Slot
                    InventoryItem invItem = new InventoryItem(i, itemNames[x]);                        
                    itemsList.Add(invItem);
                    occupiedSlots.Add(i);
                    break;
                }
            }
        }

        SaveManager.SaveItems(Name, itemsList);

        if(displayForPlayer) {
            GameObject.FindGameObjectWithTag("Managers").GetComponent<UIManager>().DisplayItems(itemNames);
        }
    }

    //Passed index is slot index, but slot index does not correspond to index of items in the list, find item with same slot index and delete it
    public void RemoveItemsFromSlots(List<int> slotIndexes) {        
      
        List<InventoryItem> itemsToRemove = new List<InventoryItem>();

        //Removing multiple items when looping through the list can mess up things
        for(int i = 0; i < itemsList.Count; i++) {
            for(int j = 0; j < slotIndexes.Count; j++) {
                if(itemsList[i].slotIndex == slotIndexes[j]) {                   
                    itemsToRemove.Add(itemsList[i]);
                }
            }         
        }

        //Remove items outside of the loop
        for(int i = 0; i < itemsToRemove.Count; i++) {
            itemsList.Remove(itemsToRemove[i]);
        }

        SaveManager.SaveItems(Name, itemsList);
    }

    // public void RemoveItemsCurse() {
    //     List<InventoryItem> currentItems = SaveManager.LoadItems(Name);

    //     if(currentItems.Count > 0) {

    //         //How many items will get remove from player. Minimum 1 tems, max is half of player's items
    //         int min = 1;
    //         int max = Mathf.Max(min, Mathf.FloorToInt( ((float)currentItems.Count)/2f) );
    //         int amountToRemove = Random.Range(min, max);
    //         print("Curse Removing "  + amountToRemove + " items");
         
    //         //Store Names to display to player what items he lost
    //         List<Items.ItemName> itemNames = new List<Items.ItemName>();
    //         //Remove items
    //         for(int i = 0; i < amountToRemove; i++) {
    //             int randomIndex = Random.Range(0, currentItems.Count);
    //             //Dont remove quest items /temporary
    //             Items.ItemName itemName = currentItems[randomIndex].itemName;                
    //             if(itemName == Items.ItemName.RecipeElderCharm || itemName == Items.ItemName.ElderCharm || itemName == Items.ItemName.ElderBerries) {
    //                 continue;
    //             }

    //             itemNames.Add(itemName);
    //             currentItems.RemoveAt(randomIndex);
    //         }
    //         //Save list of items after items were removed
    //         itemsList = currentItems;
    //         SaveManager.SaveItems(Name, currentItems);
           
    //         GameObject.FindGameObjectWithTag("Managers").GetComponent<UIManager>().DsiplayLostItems(itemNames);        
    //     }
    // }

    //Remove items by name
    //Check if all items are present, if so, remove them and save items
    public void RemoveItems(List<Items.ItemName> itemsToRemove) {

        //If only one item is not present, dont remove any items
        bool hasItems = true;
        List<InventoryItem> selectedItems = new List<InventoryItem>();

        //Go through each item, which is supposed to be removed
        for(int i = 0; i < itemsToRemove.Count; i++) {

            //Go through all itmes that character has
            bool foundItem = false;
            for(int j = 0; j < itemsList.Count; j++) {

                //Found item, add it to indexes of items to removes
                if(itemsToRemove[i] == itemsList[j].itemName) {
                    //Add item only once otherwise it would add all items of same name
                    if(!foundItem) {
                        selectedItems.Add(itemsList[j]);
                        foundItem = true;
                    }
                }
            }

            if(!foundItem) { hasItems = false; }
        }
       
        //Remove items
        if(hasItems) {
            for(int i = 0; i < selectedItems.Count; i++) {
                 itemsList.Remove(selectedItems[i]);
            }
        }

        SaveManager.SaveItems(Name, itemsList);
    }

    public void UpdateItem(int index, int SlotIndex) {
        itemsList[index].slotIndex = SlotIndex;
        SaveManager.SaveItems(Name, itemsList);
    }

    public int GetItemListIndexFromSlotIndex(int slotIndex) {

        for(int i = 0; i < itemsList.Count; i++) {
            if(itemsList[i].slotIndex == slotIndex) {
                return i;
            }
        }

        print("Could not find Item");
        return -1;
    }

    public int GetItemOwnedAmount(Items.ItemName itemName) {
        
        int amount = 0;
        for(int i = 0; i < itemsList.Count; i++) {
            if(itemsList[i].itemName == itemName) {
                amount++;
            }
        }

        return amount;
    }

    public void DayPhaseChanged(GameDirector.DayPhase dayPhase) {   
        //SetHiddenState(dayPhase);
    }

    // void SetHiddenState(GameDirector.DayPhase dayPhase) {
    //      if(isPlayer == false && Name != Characters.Names.Cauldron) {
    //         if(GetComponent<InteractionSelection>() != null) {
    //             if(dayPhase != characterDayPhase) {           
    //                 GetComponent<InteractionSelection>().IsEnabled = false;            
    //                 //Hide Character
    //                 GetComponent<SpriteAnimator>().renderObjectsParent.gameObject.SetActive(false);
    //             } else {
    //                 GetComponent<InteractionSelection>().IsEnabled = true;
    //                 //Unhide Character
    //                 GetComponent<SpriteAnimator>().renderObjectsParent.gameObject.SetActive(true);
    //             }
    //         }
    //     }
    // }

    //Dialogue
    //Save new dialogue index after dialogue was closed ( dialogueProgress )
    //Enable new dialogue only if conditions are met
    //If conditions are not met, automaticly hide the dialogue or allow only Free/NonCodnition dialogues
}
