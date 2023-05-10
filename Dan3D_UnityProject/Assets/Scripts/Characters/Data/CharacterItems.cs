using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Globalization;


//Each Character can have items on him and Dangoru can trade with him
//This script does not server as access point of character items or data, but to setup items for a character. Ie character can have some items from before
//but each npc has its own of pool items it can randomly have. It has to be loaded, selected and saved somehere, its done here. BaseData then use the saved 
//items, which were selected and saved here
public class CharacterItems : MonoBehaviour
{
    Character_BaseData baseData;

    List<PoolItem> itemsPool = new List<PoolItem>();
    List<InventoryItem> inventoryItems = new List<InventoryItem>(); 
    Dictionary<Items.ItemName, int> tradeValues = new Dictionary<Items.ItemName, int>();     
  
    public void Init()
    {
        baseData = GetComponent<Character_BaseData>();

        //LoadTradeValues();
        LoadItemsPool();
        SetupItems();

        baseData.LoadItems();
        baseData.SetTradeValues(tradeValues);
    } 

    void LoadItemsPool() {
 
        TextAsset file = (TextAsset)Resources.Load("XmlData/Items/Characters/" + baseData.Name.ToString() + "/" + baseData.Name.ToString() + "_Items");

        if(file != null) {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlItemsPool));
            XmlItemsPool items = serializer.Deserialize(new StringReader(file.text)) as XmlItemsPool;

            foreach(XmlPoolItem item in items.itemList) {
                PoolItem newItem = new PoolItem();
                newItem.itemName = ItemsDirectory.GetItemName(item.ItemName);               
                newItem.maxAmount = int.Parse(item.MaxAmount, CultureInfo.InvariantCulture);

                itemsPool.Add(newItem);
            }
        }
        else {
            Debug.Log("Items Xml File is Null");
        }   
    }

    //Setup What Items this character has from his pool of items
    void SetupItems() {
        //Random Items
        // int amountOfItems = UnityEngine.Random.Range(1, 3);
        // for(int i = 0; i < amountOfItems; i++) {
        //     //Select random item from the pool
        //     int randomItem = UnityEngine.Random.Range(0, itemsPool.Count);
        //     //Convert it to InventoryItem and add it to the list
        //     InventoryItem invItem = new InventoryItem(i, itemsPool[randomItem].itemName);
        //     inventoryItems.Add(invItem);
        // }

        //All Items
        for(int i = 0; i < itemsPool.Count; i++) {            
            //Convert it to InventoryItem and add it to the list
            InventoryItem invItem = new InventoryItem(i, itemsPool[i].itemName);
            inventoryItems.Add(invItem);
        }

        SaveManager.SaveItems(baseData.Name, inventoryItems);
    }

    // void LoadTradeValues() {

        
    //     TextAsset file = (TextAsset)Resources.Load("XmlData/Items/Characters/" + baseData.Name.ToString() + "/" + baseData.Name.ToString() + "_TradeValues");

    //     if(file != null) {
    //         XmlSerializer serializer = new XmlSerializer(typeof(XmlTradeValues));
    //         XmlTradeValues newTradeValues = serializer.Deserialize(new StringReader(file.text)) as XmlTradeValues;
           
    //         foreach(XmlTradeValue item in newTradeValues.TradeValuesList) {
    //             Items.ItemName itemName = ItemsDirectory.GetItemName(item.ItemName);                
    //             tradeValues.Add(itemName, item.Value);               
    //         }
    //     }
    //     else {
    //         Debug.Log("Items Xml File is Null");         
    //     }         
    // }
}

public class PoolItem {
    public Items.ItemName itemName;
    public int maxAmount = 1;
}

