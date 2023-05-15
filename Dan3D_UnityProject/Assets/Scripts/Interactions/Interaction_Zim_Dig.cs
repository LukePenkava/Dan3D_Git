using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class Interaction_Zim_Dig : Interaction
{
    public Zima zimSript;    

    void Awake() {
        Init();        
    }

    public override void ActivateOverride() {
        base.ActivateOverride(); 

        //zimSript.Dig();      
    }    
}
