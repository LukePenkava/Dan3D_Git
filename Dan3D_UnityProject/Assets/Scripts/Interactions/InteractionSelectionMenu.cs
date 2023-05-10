using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionSelectionMenu : MonoBehaviour
{
    public Transform parent;
    public GameObject selectionPrefab;
    public GameObject interactionTextPrefab;
    public Camera cam;
    bool isOn = false;

    InteractionSelection interaction;

    // void OnEnable() {
    //     AreaManager.AreaWillGetDeleted += AreaDeleted;
    // } 

    // void OnDisable() {
    //     AreaManager.AreaWillGetDeleted -= AreaDeleted;
    // }

    void LateUpdate() {

        if(isOn) {
            Vector3 viewPos = cam.WorldToScreenPoint(interaction.anchorSelection.position);         
            this.transform.position = viewPos;
        }
    }

    // void AreaDeleted(AreaNode areaNode) {
    //     isOn = false;
    //     parent.gameObject.SetActive(false);

    //     foreach(Transform temp in parent) {
    //         Destroy(temp.gameObject);
    //     }

    //     interaction = null;
    // }

    public void Setup(bool show, InteractionSelection Interaction) {    
        //print("Show selection Menu " + show);     
        
        isOn = show;        

        if(show == false) {
            parent.gameObject.SetActive(show);
        }
        

        foreach(Transform temp in parent) {
            Destroy(temp.gameObject);
        }

        if(show) {           
            
            interaction = Interaction;
            //Get List of Active Interactions ( update them and check which are available etc )
            List<Interaction> interactionsList = Interaction.SetActiveInteractions(true);
                               

            Vector3 viewPos = cam.WorldToScreenPoint(interaction.anchorSelection.position);         
            this.transform.position = viewPos;

            int visibleInteractions = 0;

            //Go through all interactions and create UI object for them
            for(int i = 0; i < interactionsList.Count; i++) {         

                //print("Interacdtion State " + interactionsList[i].interactionType + " " + interactionsList[i].InteractionState);               

                //If the interation state is hidden, dont show it
                if(interactionsList[i].InteractionState != InteractionStates.Hidden && interactionsList[i].InteractionState != InteractionStates.HiddenOverride) {                                        

                    GameObject newSelection = Instantiate(selectionPrefab, Vector3.zero, Quaternion.identity);
                    newSelection.transform.SetParent(parent);
                    newSelection.transform.localPosition = new Vector3(0, i * 20f, 0);      
                    newSelection.transform.localScale = Vector3.one;          
                    newSelection.GetComponent<InteractionSelectionObject>().Setup(interactionsList[i], i);

                    visibleInteractions++;

                    //Display text for Interaction in selection menu ( like "Looks like someone is hiding in the bush" )
                    if(interactionsList[i].interactionText.Length > 0) {
                        GameObject newInteractionText = Instantiate(interactionTextPrefab, Vector3.zero, Quaternion.identity);
                        newInteractionText.transform.SetParent(parent);
                        newInteractionText.transform.localPosition = new Vector3(0, 0, 0);
                        newInteractionText.transform.localScale = Vector3.one;
                        newInteractionText.GetComponent<Text>().text = interactionsList[i].interactionText;
                    }   
                }
            }
            
            parent.gameObject.SetActive(false);
            parent.gameObject.SetActive(true);

            //If all interactions are hidden or nothing was visible, altho SelectionMenu is supposed to be on, dont show it
            if(visibleInteractions == 0) {
                parent.gameObject.SetActive(false);
            }

            
            LayoutRebuilder.ForceRebuildLayoutImmediate(parent.GetComponent<RectTransform>());
            //LayoutRebuilder.ForceRebuildLayoutImmediate(this.gameObject.GetComponent<RectTransform>());
            //Canvas.ForceUpdateCanvases();
        }
        else {

            interaction = null;
        }

        
    }  
}
