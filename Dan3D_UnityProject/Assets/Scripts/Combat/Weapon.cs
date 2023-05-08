using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{   

    void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<Enemy>().GotHit();
    }
}
