using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public Items.ItemName resourceItem;
    public int amount = 4; 
    
    public GameObject visual;
    public Material mat_normal;
    public Material mat_highlight;

    public void Highlight()
    {
        Material[] mats = visual.GetComponent<MeshRenderer>().materials;
        mats[0] = mat_highlight;
        visual.GetComponent<MeshRenderer>().materials = mats;
    }
    
    public virtual void Collect() { }

}
