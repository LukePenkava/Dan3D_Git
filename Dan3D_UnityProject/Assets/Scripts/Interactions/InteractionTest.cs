using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InteractionTest : Interaction
{
    public Transform animationPos;
    public UnityEvent ActivateFunction;

    void Awake() {
        Init();
    }

    public override void ActivateOverride() {
        base.ActivateOverride();        

        print("Interaction Activated");  

        List<Items.ItemName> itemList = new List<Items.ItemName>();
        itemList.Add(Items.ItemName.Stone);                   
        player.GetComponent<Character_BaseData>().AddItems(itemList);      
    }    
}
