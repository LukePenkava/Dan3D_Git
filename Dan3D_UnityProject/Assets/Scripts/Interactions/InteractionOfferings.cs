using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InteractionOfferings : Interaction
{
    public Items.ItemName item;

    void Awake() {
        Init();
    }

    public override void ActivateOverride() {
        base.ActivateOverride();  
        
        player.GetComponent<SpiritManager>().AdjustSpirit(10);     
    }    
}
