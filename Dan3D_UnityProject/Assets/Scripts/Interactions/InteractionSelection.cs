using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If there is more Interaction on object ( trade and dialogue for example ), show selection menu for player to choose what intreaction to Trigger
//This makes it automatic to handle more interactions, but is required on all objects with interaction even if they have one interaction only
public class InteractionSelection : MonoBehaviour
{ 
    bool isEnabled = true;
    public bool IsEnabled {
        get { return isEnabled; }
        set { isEnabled = value; }
    }

    bool hasStateScript = false;
    State stateScript;
    State.States lastState;

    List<Interaction> interactions = new List<Interaction>();
    public List<Interaction> Interactions {
        get { return interactions; }
    }

    public Transform anchorSelection;

    void Awake()
    {     
        //interactionManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<InteractionManager>();

        SetActiveInteractions(false);

        //For characters to update interactions based on their state or anything else what has state
        if(GetComponent<State>()) {
            hasStateScript = true;
            stateScript = GetComponent<State>();
            lastState = stateScript.ActiveState;
        }
    }

    void Update() {
        
        if(hasStateScript) {
            if(lastState != stateScript.ActiveState) {
                lastState = stateScript.ActiveState;
                SetActiveInteractions(true);
            }
        }
    }

    //Disable SetInteraction in Awake or remove that function? This could be all that is needed
    //Called when Selection menu is supposed to appear. Update all interaction and check if they are available
    public List<Interaction> SetActiveInteractions(bool checkIfAvailable) {  

        interactions.Clear();

        //Get all Interactions on this object ( character, resource etc)
        Interaction[] tempInteractions = this.gameObject.GetComponents<Interaction>();        

        for(int i = 0; i < tempInteractions.Length; i++) {
            if(tempInteractions[i].enabled) {

                if(checkIfAvailable) {
                    //Each interaction has a function to see if its available, ie there can be conditions based on players progress etc, which disable dialogue and other things
                   
                    bool isAvailable = tempInteractions[i].SetupAndCheckIfAvailable();
                    //print(tempInteractions[i].interactionType + " is Available " + isAvailable);
                    if(isAvailable) {
                        interactions.Add(tempInteractions[i]);                
                    }
                } else {
                    interactions.Add(tempInteractions[i]);
                }
            }
        }

        //print("Amount of Interactions " + interactions.Count);

        //Currently there is collect, dialogue and trade interaction. Both Collect and Dialogue are always supposed to be first
        //If by any chance trade is first, remove it from where it is and add it at the end
        int tradeIndex = -1;
        //Get Trade Index       
        for(int i = 0; i < interactions.Count; i++) {           
            if(interactions[i].interactionType == InteractionManager.InteractionTypes.Trade) {
                tradeIndex = i;
            }
        }
        //Move Trade to the end of the list
        if(tradeIndex >= 0) {
            Interaction tradeInteraction = interactions[tradeIndex];
            interactions.RemoveAt(tradeIndex);
            interactions.Add(tradeInteraction);
        }

        return interactions;
    }

/*
    public void SetInteractions() {

        // bool isBasil = false;
        // if(GetComponent<BaseData>()) {
        //      Characters.Names name = GetComponent<BaseData>().Name;
        //      if(name == Characters.Names.Basil) { isBasil = true; }
        // }       
        // if(isBasil) { print("Set Interactions "); }

        interactions.Clear();

        //Get all Interactions on this object ( character, resource etc)
        Interaction[] tempInteractions = this.gameObject.GetComponents<Interaction>();        

        for(int i = 0; i < tempInteractions.Length; i++) {
            if(tempInteractions[i].enabled) {
                interactions.Add(tempInteractions[i]);              
            }
        }

        //Currently there is collect, dialogue and trade interaction. Both Collect and Dialogue are always supposed to be first
        //If by any chance trade is first, remove it from where it is and add it at the end
        int tradeIndex = -1;
        //Get Trade Index
        
        for(int i = 0; i < tempInteractions.Length; i++) {
            print(interactions[i].interactionType);
            if(interactions[i].interactionType == InteractionManager.InteractionTypes.Trade) {
                tradeIndex = i;
            }
        }
        //Move Trade to the end of the list
        if(tradeIndex >= 0) {
            Interaction tradeInteraction = interactions[tradeIndex];
            interactions.RemoveAt(tradeIndex);
            interactions.Add(tradeInteraction);
        }
    } */

    
}
