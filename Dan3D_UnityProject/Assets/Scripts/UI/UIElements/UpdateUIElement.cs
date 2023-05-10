using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpdateUIElement : MonoBehaviour
{
    public Text textElement;
    float textLength = 0;

    // Start is called before the first frame update
    void Awake()
    {
        textLength = textElement.text.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if(textLength != textElement.text.Length) {
            textLength = textElement.text.Length;      
            
            this.gameObject.SetActive(false);
            this.gameObject.SetActive(true);
        }
    }
}