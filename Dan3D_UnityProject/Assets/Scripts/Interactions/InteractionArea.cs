using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InteractionArea : Interaction
{
    public Areas areaToEnter = Areas.none;

    void Awake() {
        Init();
    }

    public override void ActivateOverride() {
        base.ActivateOverride();        
    
        GameObject.FindGameObjectWithTag("Managers").GetComponent<AreaManager>().LoadArea(areaToEnter);            
    }    
}
