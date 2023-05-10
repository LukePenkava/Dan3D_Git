using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class InteractionActivate : Interaction
{
    public Transform animationPos;
    public UnityEvent ActivateFunction;

    void Awake() {
        Init();
    }

    public override void ActivateOverride() {
        base.ActivateOverride();        

        ActivateFunction.Invoke();        
    }    
}
