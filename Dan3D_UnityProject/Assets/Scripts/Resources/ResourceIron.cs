using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceIron : Resource
{
    public override void Collect()
    {
        List<Items.ItemName> itemList = new List<Items.ItemName> { resourceItem };

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Character_BaseData>().AddItems(itemList, true);        
        
        amount--;

        if(amount <= 0) {
            Destroy(this.gameObject);
        }
    }
}
