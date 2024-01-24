using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Data object holding all information about given Item, exists only one time at ItemsDirectory, this is not data object to be create for items player has
//or is using. This is source of truth for those items available in ItemsDirectory
public class Item {    
   
    public Items.ItemName itemName;    
    public string title;
    public Items.ItemTags[] tags;   //Unordered array of all kinds of tags this item can have
    public bool isUsable = false;
    public Dictionary<string, string> useFunctions;  //List of uses this item has. Ie what happens when this item is used, sequence of functions
    public string description;
    public int amount = 0;

    public bool canBeActivated = true;
    public float cooldown = 0f;
    public float cooldownTimer = 0f;

    //Empty Constructor
    public Item() {}

    public Item(Items.ItemName Name, Items.ItemTags[] Tags, Dictionary<string, string> UseFunctions, int Amount) {
     
        itemName = Name;
        tags = Tags;
        useFunctions = UseFunctions;        
        amount = Amount;
    }

    public bool CheckTag(Items.ItemTags tag) {
        bool containsTag = false;
        foreach(Items.ItemTags searchTag in tags) {
            if(searchTag == tag) {
                containsTag = true;
            }
        }

        return containsTag;
    }
}
