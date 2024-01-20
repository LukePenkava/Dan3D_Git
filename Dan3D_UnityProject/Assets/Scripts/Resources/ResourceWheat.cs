using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceWheat : Resource
{ 
    public override void Collect()
    {        
        if(amount <= 0) { return; }
        amount--;

        print("In Resource Wheat");

        // List<Items.ItemName> itemList = new List<Items.ItemName>();
        // itemList.Add(Items.ItemName.Everflour);      

        // GameObject player = GameObject.FindGameObjectWithTag("Player");              
        // player.GetComponent<Character_BaseData>().AddItems(itemList, true);      
    }


}
