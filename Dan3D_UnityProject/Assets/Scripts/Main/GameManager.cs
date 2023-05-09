using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    CameraManager cameraManager;

    // Start is called before the first frame update
    void Start()
    {
        //PerformDynamicRes scaler = PerformDynamicRes.
        //DynamicResolutionHandler.SetDynamicResScaler()

        cameraManager = GetComponent<CameraManager>();
        cameraManager.Init();
    }
}
