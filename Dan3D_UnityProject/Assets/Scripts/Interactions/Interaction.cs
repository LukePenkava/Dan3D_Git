using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionStates {
        Null,
        Active,
        Disabled,       //Visible, but greyed out
        Hidden,         //Completely hidden, does not appear in SelectionMenu
        HiddenOverride  //Is set from code, not based on player state, override any player or interaction settings for state
}

//What State player has to be in in order to active/see this interaction. Ie be able to stand up only if already in sitting state
public enum StateConditions {
    Any,
    Custom,
    Sitting,
    Moving,
    Sleeping
}

//Parent Class for Generic Interaction. Base object, which can be used for any interaction
//Other scripts will talk to this script, not derived class
[RequireComponent(typeof(InteractionSelection))]
public class Interaction : MonoBehaviour
{
    public delegate void InteractionDelegate(InteractionManager.InteractionTypes interactionTyp);
    public static InteractionDelegate InteractionTriggered;

    protected Character_BaseData baseData;
    protected InteractionSelection selection;

    protected GameObject managers;
    protected GameObject player;
    protected GameDirector gameDirector;
    protected InteractionManager interactionManager;  
    //protected TimeManager timeManager; 
    //protected PlayerManager playerManager; 
    //protected SpriteAnimator playerAnimator;
    protected UIManager uiManager;
    protected State playerState;
    protected State thisState;  //If this interaction is for example character's interaction, get the character's state
    bool interactionHasState = false; //For example this is interacdtion for character and characters have its own state   

    public InteractionManager.InteractionTypes interactionType; 
    /*bool isActive = false;  //When dialogue for example, the interaction is active until turned off   
    public bool IsActive {
        get { return isActive;} 
        set { isActive = value; }
    }*/

    public GameDirector.DayPhase dayPhase = GameDirector.DayPhase.Both;
    protected bool isEnabled = true;

    InteractionData data;   
    public InteractionData Data {
        get { return data; }
        set { data = value; }
    }    

    InteractionStates interactionState = InteractionStates.Active;
    public InteractionStates InteractionState {
        get { return interactionState; }
        set {interactionState = value; }
    }
    InteractionStates prevInteractionState = InteractionStates.Active;

    public StateConditions playerStateToEnable = StateConditions.Any;   
    public StateConditions thisStateToEnable =  StateConditions.Any;
    public string customDisplayText;
    public string interactionText; //Disdplay some text in InteractionSelection menu

    bool didInit = false;

    protected void Init() {

        managers = GameObject.FindGameObjectWithTag("Managers");   
        player = GameObject.FindGameObjectWithTag("Player");   
        gameDirector = managers.GetComponent<GameDirector>();
        interactionManager = managers.GetComponent<InteractionManager>();
        uiManager = managers.GetComponent<UIManager>();
        //timeManager = managers.GetComponent<TimeManager>();
        //playerManager = player.GetComponent<PlayerManager>();
        //playerAnimator = player.GetComponent<SpriteAnimator>();
        playerState = player.GetComponent<State>();
        baseData = GetComponent<Character_BaseData>();
        selection = GetComponent<InteractionSelection>();

        if(GetComponent<State>()) {
            interactionHasState = true;
            thisState = GetComponent<State>();
        }

        data = interactionManager.interactionsData[interactionType]; 
        //DayPhaseChanged(GameDirector.dayPhase);
        didInit = true;   
    }

    //If want to use OnEnable/OnDisable in interaction inhereted class, override these and call base.OnEnable()
    // protected virtual void OnEnable() {        
    //     TimeManager.DayPhaseChanged += DayPhaseChanged;
    // }

    // protected virtual void OnDisable() {
    //     TimeManager.DayPhaseChanged -= DayPhaseChanged;
    // }

    //Called from InteractionSelection right before interaction selection menu appears.
    //Use to setup interaction or hide it if needed 
    public virtual bool SetupAndCheckIfAvailable() {       
        //Can be overriden by child class to disable availability in certain conditions
        //First check isEnabled (set based on dayPhase), if its wrong dayphase, dont even check as interaction is not supposed to be on due to dayphase
        //DayPhaseChanged(GameDirector.dayPhase);
        return isEnabled;
    }

    public void Activate() {

        //Main Function of the Interaction is handled in its child class
        ActivateOverride();

        if(data.interactionLife == InteractionManager.InteractionLife.Consumable) {   
            Destroy(this.gameObject);            
        }
    }

    //Scripts talk to Interaction script, this script, not to interactionTrade or InteractionDialogue. They can to talk to this script and use Activate(), which calls this, which is implemented by child class
    //This way any type of interaction can be called through Interaction parent class and calling Activate()
    public virtual void ActivateOverride() {    
        //Implement function in child class    

         if (InteractionTriggered != null) {            
            InteractionTriggered(interactionType);
        }
    }   

    public virtual void Close() {
        
    }

    //If Dangoru needs to walk to the interaction before it starts, use this. Pass position where he needs to walk and then delegate which gets called, once he gets there
    // public virtual void NavigateToInteraction(Vector3 pos, CharacterAI.NavigationCompletedDelegate navigatedDelegate) {
      
    //     //Go to AI script on Player and pass it pos to navigate and delegate
    //     playerManager.AllInputEnabled = false;
    //     player.GetComponent<CharacterAI>().SetNavigate(pos, navigatedDelegate, true);
    // }

    void DayPhaseChanged(GameDirector.DayPhase activeDayPhase) {
        if(dayPhase == GameDirector.DayPhase.Both || dayPhase == activeDayPhase) {
            isEnabled = true;
        } else {
            isEnabled = false;
        }        
    }    

    void Update() {

        if(didInit == false) { return; }

        if(prevInteractionState != interactionState) {
            
            // if(interactionHasState) {
            //     print(interactionType + " " + this.gameObject.name + " curState " + interactionState + " prevState " + prevInteractionState + " CharacterState " + thisState.ActiveState);
            // }

            prevInteractionState = interactionState;  

            //If this is a character, dont update selection menu if he is doing something, only update it when he is not talking, interacting etc            
            if(interactionHasState) {
                if(thisState.ActiveState != State.States.Interacting) {
                    interactionManager.CheckSelectionMenuVisibility(selection); 
                }    
            } else {                
                interactionManager.CheckSelectionMenuVisibility(selection);
            }
        }      

        //print(interactionType + " " + interactionState + " " + playerState.ActiveState + " " + playerStateToEnable);

        //If hidden override, dont set state based on anything, this is set from code and is hiding this interaction until code changes it back
        if(interactionState == InteractionStates.HiddenOverride) {
            return; 
        } 

        //Player State enabling/disabling interactions. Ie if player is sitting enable Stand up interaction. If he is Standing, enable sit interaction. both on same object, but enabled/disable at correct player state
        if(playerStateToEnable == StateConditions.Any || playerStateToEnable == StateConditions.Custom) {
            //interactionState = InteractionStates.Active;
        } else {

            //General conditions to enable interaction. Ie when enable this interaction only if player/Dangoru is sitting
            // if(playerStateToEnable == StateConditions.Sitting) {
            //     if(playerState.ActiveState == State.States.Sitting) {
            //         interactionState = InteractionStates.Active;
            //     } else {
            //         interactionState = InteractionStates.Hidden;
            //     }
            // }

            // if(playerStateToEnable == StateConditions.Sleeping) {
            //     if(playerState.ActiveState == State.States.Sleeping) {
            //         interactionState = InteractionStates.Active;
            //     } else {
            //         interactionState = InteractionStates.Hidden;
            //     }
            // }

            if(playerStateToEnable == StateConditions.Moving) {
                if(playerState.ActiveState == State.States.Walking || playerState.ActiveState == State.States.Idle || playerState.ActiveState == State.States.Running) {
                    interactionState = InteractionStates.Active;
                } else {
                    interactionState = InteractionStates.Hidden;
                }
            }
        }

        //Enable/Disable interacdtions based on this interacdtions/characters state
        if(interactionHasState) {
            if(thisStateToEnable == StateConditions.Any) {
                //interactionState = InteractionStates.Active;
            } else {
                // if(thisStateToEnable == StateConditions.Sleeping && thisState.ActiveState == State.States.Sleeping ) {
                //     interactionState = InteractionStates.Active;
                // } else {
                //     interactionState = InteractionStates.Hidden;
                // }

                if(thisStateToEnable == StateConditions.Moving) {
                    if(thisState.ActiveState == State.States.Walking || thisState.ActiveState == State.States.Idle || thisState.ActiveState == State.States.Running) {
                        interactionState = InteractionStates.Active;
                    } else {
                        interactionState = InteractionStates.Hidden;
                    }
                }
            }
        }
        
    }

   
}
