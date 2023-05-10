using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    void Awake()
    {
        GameObject.FindGameObjectWithTag("Managers").GetComponent<CameraManager>().SetZone(this.transform);   
    }    
}