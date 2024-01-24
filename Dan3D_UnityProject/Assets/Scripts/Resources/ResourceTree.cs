using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceTree : Resource
{
    public Material mat_normal;
    public Material mat_highlight;

    public GameObject treeVisual;

    int amount = 4;

    public override void Highlight()
    {
        Material[] mats = treeVisual.GetComponent<MeshRenderer>().materials;
        mats[0] = mat_highlight;
        treeVisual.GetComponent<MeshRenderer>().materials = mats;
    }

    public override void Collect()
    {
        List<Items.ItemName> itemList = new List<Items.ItemName> { resourceItem };

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<Character_BaseData>().AddItems(itemList, true);
    }
}
