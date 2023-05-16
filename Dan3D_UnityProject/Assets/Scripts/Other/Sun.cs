using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : MonoBehaviour
{
    public Vector3 start;
    public Vector3 end;
    Vector3 curState;
    public float duration;
    float timer = 0f;

    public Transform sun;
    
    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > duration) {
            timer = 0f;
        }
        float interpolationValue = timer / duration;

        curState = Vector3.Lerp(start, end, interpolationValue);

        sun.transform.rotation = Quaternion.Euler(curState.x, curState.y, curState.z);
    }
}
