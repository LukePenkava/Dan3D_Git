using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceWood : Resource
{
    int amount = 2;

    public Animation anim;
    
    public override void Collect()
    {
        if(amount <= 0) { return; }
        amount--;

        List<Items.ItemName> itemList = new List<Items.ItemName>();
        itemList.Add(Items.ItemName.Wood);      

        GameObject player = GameObject.FindGameObjectWithTag("Player");              
        player.GetComponent<Character_BaseData>().AddItems(itemList, true);

        anim.Stop();
        anim.Play("ResourceShake");
    }
}
