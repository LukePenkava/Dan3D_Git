using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceCollider : MonoBehaviour
{
    public Resource res;
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Weapon") {
            res.Collect();
        }        
    }
}
