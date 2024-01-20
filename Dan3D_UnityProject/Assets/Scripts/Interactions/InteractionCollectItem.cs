using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InteractionCollectItem : Interaction
{
    public Items.ItemName item;

    void Awake() {
        Init();
    }

    public override void ActivateOverride() {
        base.ActivateOverride();     

        print("In Interaction Collect");   

        List<Items.ItemName> itemList = new List<Items.ItemName>();
        itemList.Add(item);                   
        player.GetComponent<Character_BaseData>().AddItems(itemList, true);      
    }    
}
