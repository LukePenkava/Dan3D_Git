using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

//Interaction for Collecting Resources, like plants, wood etc. Using Collect type of Interaction
public class InteractionResource : Interaction
{  
    /*
    ResourceManager resourceManager;
    InteractionCheer interactionCheer;

    public Items.ItemName itemName;
    public Items.ItemName[] itemsToReceive; //Give player either one item, or one random item from the list
    public int resourceIndex = 0; //Has to be unique for each Resource, so it can be saved. Use itemName_areaIndex_resourceIndex. So thats its not necessary to keep track of all resources around all areas. This index has to be unique only for the area  
    public Transform anchor;
    public Transform anchorLife;
    public SpriteRenderer visual;
    public Animations.Animation gatherAnimation = Animations.Animation.Interaction;
    public bool dailyResource = false;     //Dont use resource life, just collect it
    public bool canBeCheered = true;
    public bool customGathering = false;
    public bool useCompletedEvent = false;    //Uses CollectingCompleted Event to trigger some function outside of this script ( like Stardew )
    public bool navigateToResource = false;
    public bool useDialogue = true;
    public bool hasGlow = false;

    public MgManager.MgTypes mgType;  
    public int difficulty; 

    public enum ResourceStates {
        Null,
        Empty,
        Start,
        Mid,
        Full
    }    

    ResourceStates resourceState = ResourceStates.Null;
    public ResourceStates ResourceState {
        get { return resourceState; }
    }

    ResourceStates lastResourceState = ResourceStates.Null;
   
    public GameDirector.DayPhase resourceDayPhase = GameDirector.DayPhase.Day;    
    
    float life = 0.0f; //Goest from 0 to lifeCycle, ie 0 - 100, not 0.0 - 1.0
    public float Life {
        get { return life; }
        set { life = value; }
    }
    
    public float lifeCycle = 100.0f;     //Time it takes for a resource to get into Full state    
    public float valueStart = 0.25f;
    public float valueMid = 0.7f;         //At what parameter the Resource goes to mid state on 0.0-1.0
    float valueFull = 1.0f;
    public float cheerValue = 0.3f;
    public int itemAmountMid = 1;       //Amount of items player gets from mid state
    public int itemAmountFull = 2;      //Amount of itmes player gets from full state

    bool isHidden = false;  public bool IsHidden { get { return isHidden; } }
    bool rolledOnHidden = false;  public bool RolledOnHidden { get { return rolledOnHidden; } set { rolledOnHidden = value; } }

    bool dayPhaseHidden = false; 
    public bool DayPhaseHidden {
        get { return dayPhaseHidden; } 
    }

    public float anchorLifePosY = 0f;

    bool mgOn = false;  //If minigame is on, dont update the state as it is handled outise of ResourceManager
    public bool MgOn {
        get { return mgOn; }
    }

    //Dialogue
    DialogueManager dialogueManager;
    Dictionary<int, DialogueObject> dialoguesData; 
    int lineIndex = 0;
    int dialogueIndex = 0;
   
    //Events   
    public UnityEvent ActivateEvent;
    public UnityEvent<bool> collectingCompleted;
    bool mgSuccess = false;


    //Walk towards the resource, play Dan's animation


   
    void Awake()
    {
        Init(); 

        GameObject managers = GameObject.FindGameObjectWithTag("Managers");
        dialogueManager = managers.GetComponent<DialogueManager>();
        resourceManager = managers.GetComponent<ResourceManager>();
        dialoguesData = dialogueManager.LoadDialogues(Characters.Names.Plant);
        interactionCheer = GetComponent<InteractionCheer>();
        anchorLife.transform.localPosition = new Vector3(anchorLife.transform.localPosition.x, anchorLifePosY, anchorLife.transform.position.z);

        //SetDayPhase(gameDirector.dayPhase);
    }

    public override void ActivateOverride() {
        base.ActivateOverride();

        if(navigateToResource) {
            base.NavigateToInteraction(anchor.position, ActiveResource);
        } else {
            ActiveResource();
        }
    }

    public void ActiveResource() {
        if(interactionType == InteractionManager.InteractionTypes.MG) {

            MgManager mgManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<MgManager>();       
            mgManager.InitMG(mgType, this, difficulty, gatherAnimation);
            mgOn = true;

        } else if(interactionType == InteractionManager.InteractionTypes.CollectQuick) {
            //Give items to player
            StartCoroutine(WaitForUI(true, 1, 0.4f));  
            life = 0f;
            SetState();           
            resourceManager.SaveResources();                            
        }        
    }


    public void SetDayPhase(GameDirector.DayPhase dayPhase) {   
        //print(this.gameObject.name + " " + dayPhase);

        if(dayPhase != resourceDayPhase) {           
            GetComponent<InteractionSelection>().IsEnabled = false;
            InteractionState = InteractionStates.HiddenOverride;
            dayPhaseHidden = true;
            visual.sprite = null;
            if(hasGlow) {
                visual.GetComponent<SpriteRenderer>().material.SetTexture("_Emission", null);
            }
        } else {
            GetComponent<InteractionSelection>().IsEnabled = true;
            dayPhaseHidden = false;
            SetState(true, true);
        }
    }
    

    public void SetState(bool setVisual = true, bool overrideState = false) {
        //print(this.gameObject.name + " " + setVisual + " " + overrideState);
        if(dayPhaseHidden) { return; }
        
        if(dailyResource) {  

            if(life > 0) {
                InteractionState = InteractionStates.Active;
                resourceState = ResourceStates.Full;
            } else {
                InteractionState = InteractionStates.HiddenOverride;
                resourceState = ResourceStates.Empty;
            }    

        } else {

            float lifeParameter = life / lifeCycle; //Convert time to 0.0-1.0 parameter      

            if(lifeParameter >= 0 && lifeParameter < valueStart){
                resourceState = ResourceStates.Empty;   
                InteractionState = InteractionStates.HiddenOverride;

            } else if (lifeParameter >= valueStart && lifeParameter < valueMid) {
                resourceState = ResourceStates.Start;
                InteractionState = InteractionStates.HiddenOverride;

            } else if (lifeParameter >= valueMid && lifeParameter < valueFull) {
                resourceState = ResourceStates.Mid;
                InteractionState = InteractionStates.Active;

            }
            else {
                resourceState = ResourceStates.Full;
                InteractionState = InteractionStates.Active;
            }
        }


        if( (resourceState != lastResourceState) || overrideState) {
            lastResourceState = resourceState;

            //Set Visual
            if(setVisual) {
                string visualState = resourceState.ToString();
                if(resourceState == ResourceStates.Null) { visualState = ResourceStates.Empty.ToString(); } //Dont have visual state for Null
                //print("Visual State " + visualState);         
                string spriteName = "ResourceObjects/" + itemName.ToString() + "/" + itemName.ToString() + "_" + visualState; 
                //print(this.gameObject.name + " " + spriteName);      
                visual.sprite = Resources.Load<Sprite>(spriteName);
                if(hasGlow) {
                    Texture glow_Texture = Resources.Load<Texture>(spriteName + "_Glow");
                    visual.GetComponent<SpriteRenderer>().material.SetTexture("_Emission", glow_Texture);                    
                }
            }
        }
    }

    public int AmountOfItemsToReceive() {

        int amount = 0;

        if(resourceState == ResourceStates.Mid) { amount = itemAmountMid; }
        if(resourceState == ResourceStates.Full) { amount = itemAmountFull; }

        return amount;
    }

    public void Cheer() {
        life += cheerValue;        
    }

    public void SetResouceLife(float lifeValue, float saveTime) {          
        float timeToAdd = GameDirector.GameTime - saveTime;      

        if(timeToAdd < 0f) {
            print("RESOURCE LIFE ERROR: GameTime " + GameDirector.GameTime + " SaveTime " + saveTime + " LifeValue " + lifeValue);
            timeToAdd = 0F;
        }

        life = lifeValue + timeToAdd; 
        if(dailyResource) { life = lifeValue; }
        //print(itemName + " life " + life + " lifeValue " + lifeValue + " saveTime " + saveTime + " timeToAdd " + timeToAdd);

        SetState();
    }

    //At the end of MG for any automated resoruces
    public void Reset(bool wasSuccess) {
        if(dailyResource) { return; }        

        life = 0;
        
        SetState(); //Dont set visual if there is animation to be played at the end ( like stardew )
        if(useDialogue) {
            DisplayDialogueLine(wasSuccess);
        }
        mgOn = false;        

        //If there is some custom logic at the end of collection, invoke the method assigned in the editor
        if(useCompletedEvent) {
            collectingCompleted.Invoke(wasSuccess);
        }        
    }

    //Called at the end of MG for any resource with custom customGathering == true
    //Add player items in this script, all that logic is skipped there, assuming there is for example animation to play and after that give player items
    public void MgCompleted(bool wasSuccess) {                

        //CustomGathering ignores normal logic and expects it to be implement in collectingCompeted event
        if(customGathering) {          
            mgSuccess = wasSuccess; //Used later in CustomLogicCompleted           
        }
        //Default logic for completed MG 
        else {    
            if(wasSuccess) {           
                int amountOfItems = AmountOfItemsToReceive();
                StartCoroutine(WaitForUI(wasSuccess, amountOfItems, 0f));           
            }

            //Reset this Resource
            Reset(wasSuccess);
            if(canBeCheered) {
                interactionCheer.SetState();    
            }
            resourceManager.SaveResources();

            //Play Animation
            AnimSettings anim1 = new AnimSettings(Animations.Animation.Idle, Animations.AnimationType.Loop, true, State.States.Idle);  
            playerAnimator.SetAnimations(anim1);

            playerManager.InteractionFinished(false);          
            //Change state back
            GameDirector.gameState = GameDirector.GameState.World;
        }    

        if(useCompletedEvent) {
            collectingCompleted.Invoke(wasSuccess);
        }  
    }

    //This is same logic as in MgManager, but here it can be called at some other time when needed from other script ( like Stardew animation finished )
    public void CustomLogicCompleted() {       

        //Display UI with delay, call it before, because values gets reseted here after this call        
        int amountOfItems = AmountOfItemsToReceive();
        StartCoroutine(WaitForUI(mgSuccess, amountOfItems)); 

        life = 0;        
        SetState(); //Dont set visual if there is animation to be played at the end ( like stardew )
        mgOn = false;
        mgSuccess = false;
        if(canBeCheered) {
            GetComponent<InteractionCheer>().SetState();    
        }
        resourceManager.SaveResources();         

        playerManager.InteractionFinished(false);          
        //Change state back
        GameDirector.gameState = GameDirector.GameState.World;                 
    }    


    IEnumerator WaitForUI(bool giveItems, int amountOfItems, float timer = 0.4f) {
        yield return new WaitForSeconds(timer);      
        
        if(useDialogue) {
            DisplayDialogueLine(giveItems);     
        }

        if(giveItems) {
            //Give items to player             
            int rndIndex = Random.Range(0, itemsToReceive.Length);
            Items.ItemName chosenItem = itemsToReceive[rndIndex];            

            List<Items.ItemName> items = new List<Items.ItemName>();
            //Get Resource state to find out how many items player gets            
            for(int i = 0; i < amountOfItems; i++) {
                items.Add(chosenItem);
            }        
            BaseData playerData = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseData>();        
            playerData.AddItems(items, true);
        }  

        if(ActivateEvent != null) {
            ActivateEvent.Invoke();
        }
    }    

    public void DisplayDialogueLine(bool wasSuccess) {
        string matchType = wasSuccess ? "Success" : "Fail"; 
        string dialogueType = "";   
        int attempts = 0;
        lineIndex = 0;

        while(matchType != dialogueType || dialogueIndex == Director.lastPlantDialogueIndex) {
            int randomIndex = Random.Range(0, dialoguesData.Count);     
            dialogueIndex = randomIndex;
            dialogueType = dialoguesData[dialogueIndex].dialogueLines[lineIndex].type;
            
            if(attempts > 100) { 
                Debug.Log("Display Dialogue Line out of attempts"); 
                break;
            }
            attempts++;
        }

        Director.lastPlantDialogueIndex = dialogueIndex;

        DialogueObject dialogueObj = dialoguesData[dialogueIndex];               
        dialogueManager.DisplayText(anchor, dialogueObj, lineIndex);
        //string line = dialoguesData[dialogueIndex].dialogueLines[lineIndex].line; 
        //dialogueManager.DisplayText(line, dialoguesData[dialogueIndex].dialogueLines[lineIndex].character, dialoguesData[dialogueIndex].type, anchor, 0); 
        StartCoroutine(CloseDialogue());
    }  

    IEnumerator CloseDialogue() {
        yield return new WaitForSeconds(Director.displayItemTime);
        dialogueManager.CloseDialogue(true, null);
    }

    public void SetHidden() {        
        life = 0;
        SetState();
        isHidden = true;
    }
    */
}
