using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InteractionCook : Interaction
{
    void Awake() {
        Init();
    }

    public override void ActivateOverride() {
        base.ActivateOverride();        

        GameObject.FindGameObjectWithTag("Managers").GetComponent<GameDirector>().Cook();      
    }    
}
