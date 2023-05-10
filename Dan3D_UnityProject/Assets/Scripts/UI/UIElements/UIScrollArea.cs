using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIScrollArea : MonoBehaviour, IDragHandler
{  
    UIManager uiManager;

    public RectTransform dragObject;  
    public RectTransform ScrollAreaRect {
        get { return this.GetComponent<RectTransform>(); }
    }

    float topLimit = 0;
    float bottomLimit = 0;

    float scrollStep = 0f;
    float height;

    //Smoothing
    float moveVelocity;
    float previousIndexedPosition = 0f;
    float targetPos = 0f;

    public Vector2 SetGaps(float gapSize) {

        ScrollAreaRect.offsetMin = new Vector2(gapSize, gapSize);
        ScrollAreaRect.offsetMax = new Vector2(-gapSize, -gapSize);
        float scrollAreaWidth = ScrollAreaRect.rect.width - gapSize;
        float scrollAreaHeight = ScrollAreaRect.rect.height; 

        return new Vector2(scrollAreaWidth, scrollAreaHeight);
    }

    //This is not setting Scroll area size in any way. Scroll area size is stretching child to fit its parent.
    //TopLimit and BottomLimit limits where scroll area can move
    //Scrollstep and Height defines how much scrollarea moves when using keys
    public void Init(float TopLimit, float BottomLimit, float ScrollStep, float Height, float gap) {
        uiManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<UIManager>();

        topLimit = TopLimit;
        bottomLimit = BottomLimit;
        scrollStep = ScrollStep;

        Vector3 pos = dragObject.anchoredPosition;
        dragObject.anchoredPosition = new Vector3(pos.x, 0f, pos.z);

        height = Height;

        previousIndexedPosition = 0;
        targetPos = 0;

        //When Scrolling, to make it look better dont have scrolling go over the edge, but have some padding. For that the scroll area has to be offseted from actual borders. Set it based on gapBetweenItems      
        //ScrollAreaRect.offsetMin = new Vector2(gap, gap);
        //ScrollAreaRect.offsetMax = new Vector2(-gap, -gap);
    }

    public void OnDrag(PointerEventData eventData) {
        
        MoveScrollArea(eventData.delta.y);
    }

    public void MoveScrollArea(float amount) {

        Vector3 pos = dragObject.anchoredPosition;
        dragObject.anchoredPosition = new Vector3(pos.x, pos.y + amount, pos.z);

        if(dragObject.anchoredPosition.y > topLimit) {          
            dragObject.anchoredPosition = new Vector3(pos.x, topLimit, pos.z);
        }

        if(dragObject.anchoredPosition.y < bottomLimit) {
           dragObject.anchoredPosition = new Vector3(pos.x, bottomLimit, pos.z);
        }
    }

    //Called from UIManager NavigateUI, sends position of selected element 
    public void AdjustPosition(Vector2 pos) {

        //Amount of slots on screen
        int steps =  Mathf.RoundToInt(height / scrollStep);   
        //Start is at 0, then moving down to negative values, first pos is half of step
        float startPosY = scrollStep / -2f;
        //Get Vertical Index of selected Items. Index starts at 0. If there is for example 5 steps, index goes from 0 to 4
        int verticalIndex = Mathf.RoundToInt((pos.y - startPosY) / scrollStep) * -1;       
        //Position of the scorlling area ( draggable object )
        Vector3 areaPos = dragObject.anchoredPosition;        
        
        //Find Active Indes Window based on area pos. Area pos is for example 0, then bottom index is 0, if pos is 300 ( 150 for step ), then bottom index is 2
        int activeWindowBottomIndex = Mathf.RoundToInt(areaPos.y / scrollStep);
        int activeWindowTopIndex = activeWindowBottomIndex + (steps -1);
        //print("Vertical Index " + verticalIndex + " Top Index " + activeWindowTopIndex + " Bottom Index " + activeWindowBottomIndex);

        //Move Area only when selected item is outside of active window. Ie when selected position moves in visible slots, dont do anything
        //print("VerticalIndex " + verticalIndex + " activeWindowTopIndex " + activeWindowTopIndex + " activeWindowBottomIndex " + activeWindowBottomIndex);
        if(verticalIndex > activeWindowTopIndex || verticalIndex < activeWindowBottomIndex) {       

            float indexedPosition =  0; 
            //Find out where to move the area/how much. Get Bottom Index ( which is at top on screen, not bottom ) and use it as actual index wher to move the area.
            //When we are here, it means selected position is outside of the view. Always only by one only, so we can move the position by 1 step only
            if(verticalIndex > activeWindowTopIndex) {
                indexedPosition = (activeWindowBottomIndex + 1) * scrollStep;
            } else {
                indexedPosition = (activeWindowBottomIndex - 1) * scrollStep;
            }
            //Round it to clean up the value
            indexedPosition = Mathf.Round(indexedPosition);
            
            targetPos = indexedPosition;
            StopCoroutine("MoveArea");
            StartCoroutine("MoveArea");
            previousIndexedPosition = indexedPosition;

        } else {
            //Do Nothing
        } 

        //print("Height " + height);
        //print("Step " + scrollStep);
        //print("Steps " + steps);
        //print("Pos " + pos);
        //print("Vertical Index " + verticalIndex);
        //print("Area Pos " + areaPos);
    }


    IEnumerator MoveArea() {

        float dampedPos = previousIndexedPosition;
        float target = targetPos;
        
        float time = 0;
        float duration = 1.0f;
        float t = 0;

        while(t < 0.99f) {

            time += Time.deltaTime;
            t = time / duration;
            if(t > 0.99f) { t = 1f; }

            //print("Indexed Pos " + targetPos + " Smoothed " + smoothedIndexedPosition);
            dampedPos = Mathf.SmoothDamp(dampedPos, targetPos, ref moveVelocity, 0.1f);
            dragObject.anchoredPosition = new Vector3(dragObject.anchoredPosition.x, dampedPos);
            yield return null;
        }
    }

    
}
