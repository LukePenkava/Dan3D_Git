using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIElement : MonoBehaviour
{
    public Canvas canvas;
    public bool inScrollArea = false;
    public RectTransform scrollArea;

    //If it is part of scrollArea, then flag it as object inside scroll area and based on the position and size of scroll are and position of this element figure out if it is in or outside of scroll area
    //Check position of this element if it is outside of Screen, then its not visible

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 elementPos = new Vector2(transform.position.x / canvas.scaleFactor, transform.position.y / canvas.scaleFactor);
        Vector2 scrollAreaPos = new Vector2(scrollArea.transform.position.x / canvas.scaleFactor, scrollArea.transform.position.y / canvas.scaleFactor);
        Vector2 scrollAreaWidth = scrollArea.sizeDelta;

        Vector2 topLeft = new Vector2(scrollAreaPos.x - scrollArea.rect.width/2f, scrollAreaPos.y + scrollArea.rect.height/2f);
        //print("Width " + scrollArea.rect.width + " " + topLeft);

       // print(elementPos);
        //print(scrollAreaPos);
    }
}
