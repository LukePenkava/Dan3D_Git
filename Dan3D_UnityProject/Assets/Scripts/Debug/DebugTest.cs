using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DebugManager.Instance.Debug_ValueWithPosition("#Test12", "Distance", 1f, new Vector2(0f, 50f), this.transform.position);
    }
}
