using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    private static DebugManager _instance;
    public static DebugManager Instance { get { return _instance; } }

    public bool debugOn = true;

    Camera mainCamera;
    GUIStyle style;

    Dictionary<string, DebugObject_ValueWithPos> DebugDictionary_ValuesWithPosition = new Dictionary<string, DebugObject_ValueWithPos>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);            
        } else {
            _instance = this;

            style = new GUIStyle();
            style.fontSize = 24;
            style.normal.textColor = Color.white;

            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        }
    }   

    void OnGUI() {      

        if(debugOn) {
            foreach(KeyValuePair<string, DebugObject_ValueWithPos> debugObj in DebugDictionary_ValuesWithPosition) {
                Vector3 screenPos = mainCamera.WorldToScreenPoint(debugObj.Value.worldPosition);
                float width = 160f;
                float height = 20f;
                GUI.Label(new Rect(screenPos.x - width/2f + debugObj.Value.uiOffset.x, Screen.height - screenPos.y - height/2f - debugObj.Value.uiOffset.y, width, height), debugObj.Value.valueName + " " + debugObj.Value.debugValue.ToString("f2"), style);
            }
        }
    }    

    public void Debug_ValueWithPosition(string debugKey, string valueName, float value, Vector2 uiOffset, Vector3 pos) {        
        DebugObject_ValueWithPos obj = new DebugObject_ValueWithPos(valueName, value, uiOffset, pos);
        DebugDictionary_ValuesWithPosition[debugKey] = obj;      
    }
}

class DebugObject_ValueWithPos {
    public string valueName;
    public float debugValue;
    public Vector2 uiOffset;
    public Vector3 worldPosition;

    public DebugObject_ValueWithPos(string _valueName, float _value, Vector2 _offset, Vector3 _pos) {       
        valueName = _valueName;
        debugValue = _value;
        uiOffset = _offset;
        worldPosition = _pos;

    }
}
