using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Globalization;

//Keeps all data for Items. Static class, can be accesed from anywere to get item data info
public static class ItemsDirectory
{
    public static Dictionary<Items.ItemName, Item> ItemsData = new Dictionary<Items.ItemName, Item>();

    public static void Init() {
        LoadItemsData();
    }

    static void LoadItemsData() {
        
        ItemsData.Clear();
        TextAsset file = (TextAsset)Resources.Load("XmlData/Items/ItemsData");

        if(file != null) {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlItems));
            XmlItems items = serializer.Deserialize(new StringReader(file.text)) as XmlItems;

            foreach(XmlItem item in items.itemList) {
                Item newItem = new Item();
                newItem.itemName = GetItemName(item.Name);

                //Set tags array by converting strings to enums
                Items.ItemTags[] tagsArray = new Items.ItemTags[item.Tags.Length];
                for(int i = 0; i < item.Tags.Length; i++) {
                    Items.ItemTags tag = (Items.ItemTags)Enum.Parse(typeof(Items.ItemTags), item.Tags[i]);
                    tagsArray[i] = tag;
                }
                newItem.tags = tagsArray;              

                //Setup Uses Dictionary
                Dictionary<string, string> useFunctions = new Dictionary<string, string>();
                if(item.UseFunctions.Length > 0) {
                    foreach(XmlUseFunction useFunction in item.UseFunctions) {
                        //Debug.Log(item.Title + " " + useFunction.FunctionName + " " + useFunction.Value);
                        useFunctions.Add(useFunction.FunctionName, useFunction.Value);
                    }
                }
                newItem.useFunctions = useFunctions;
                newItem.isUsable = (useFunctions.Count > 0) ? true : false;

                newItem.title = item.Title;
                newItem.description = item.Description;
                newItem.amount = int.Parse(item.Amount, CultureInfo.InvariantCulture);

                newItem.canBeActivated = true;
                newItem.cooldown = 10f;
                newItem.cooldownTimer = 0f;

                ItemsData.Add(newItem.itemName, newItem);
            }            
        }
        else {
            Debug.Log("Items Xml File is Null");
        }
    }

    public static Items.ItemName GetItemName(string val) {

        return (Items.ItemName)Enum.Parse(typeof(Items.ItemName), val, true);
    }
}
