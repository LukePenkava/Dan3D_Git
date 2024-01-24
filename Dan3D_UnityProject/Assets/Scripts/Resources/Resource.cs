using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public Items.ItemName resourceItem;

    public virtual void Highlight() { }
    
    public virtual void Collect() { }

}
