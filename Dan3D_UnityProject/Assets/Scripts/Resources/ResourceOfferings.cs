using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceOfferings : Resource
{ 
    public override void Collect()
    {        
        if(amount <= 0) { return; }
        amount--;     

        //GameObject player = GameObject.FindGameObjectWithTag("Player");              
        
        print("In Resource Offerings");     
    }


}
