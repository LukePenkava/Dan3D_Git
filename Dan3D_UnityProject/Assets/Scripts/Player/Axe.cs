using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Tree")
        {
            other.gameObject.GetComponent<Resource>().Collect();
        }
    }

}

