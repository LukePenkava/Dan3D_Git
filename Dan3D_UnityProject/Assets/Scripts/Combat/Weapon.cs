using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{   

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy") {
            other.gameObject.GetComponent<Enemy>().GotHit();       
        }
    }
}
