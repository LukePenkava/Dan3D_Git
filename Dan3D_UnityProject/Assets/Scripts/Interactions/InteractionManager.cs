using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public GameObject player;
    PlayerManager playerManager;

    //Interaction activeInteraction;
    InteractionSelection activeInteraction;
    int selectedInteraction = 0;
    List<InteractionSelection> sceneInteractions = new List<InteractionSelection>();
    public List<InteractionSelection> SceneInteractions
    {
        get { return sceneInteractions; }
    }
    List<GameObject> sceneCharacters = new List<GameObject>();

    public Dictionary<InteractionTypes, InteractionData> interactionsData = new Dictionary<InteractionTypes, InteractionData>();

    public InteractionSelectionMenu selectionMenu;

    //Used to connect Data information from interactionsData to specific Interaction. Used as a selector
    public enum InteractionTypes
    {
        Collect,
        Consume,
        Dialogue,
        Trade,
        Activate,
        MG,
        Cheer,
        CauldronCraftingReady,
        CauldronCraftingStart,
        Sit,
        Stand,
        Sleep,
        WakeUp,
        Eat,
        HidingCharacter,
        CollectQuick,
        GiveItem
    }

    public enum InteractionLife
    {
        NotSet,
        Consumable,         //Is Activated and then Destroyed, like gathering plant
        Single,             //Is one time activatio, which can be activated again, like one line dialogues
        ActiveForTime,      //Interaction is alive for period of time
        Active              //Is in Active state until turned off. Like Trading   
    };

    //Defines player ability to move
    public enum InteractionPlayerStates
    {
        NotSet,
        Free,               //Can move freely during interaction, like one line dialogues
        LockedForTime,      //Cant move for a period of time, like gathering
        Locked              //Locked fully, cant do anything, until deactivated
        //LockedMovement      //Can do anything except for moving
    }

    void OnEnable()
    {
        AreaManager.AreaLoaded += AreaLoaded;
    }

    void OnDisable()
    {
        AreaManager.AreaLoaded -= AreaLoaded;
    }

    public void Init()
    {

        playerManager = player.GetComponent<PlayerManager>();
        SetupInteractionsData();
        //FindInteractions();
        selectionMenu.gameObject.SetActive(true);
        ShowSelectionMenu(false);
    }

    void AreaLoaded(Area areaScript)
    {
        FindInteractions();
        // selectionMenu.gameObject.SetActive(true);
        // ShowSelectionMenu(false);
    }

    void DayPhaseChanged(GameDirector.DayPhase dayPhase)
    {
        //FindInteractions();
    }

    //Go through all found interactions in sceneInteractions, get closest one and if player is close enough, show SelectionMenu
    void Update()
    {
        //If there is active interaction, dont look for new one
        if (playerManager.IsInteracting) { return; }

        //print("Interactions " + sceneInteractions.Count); 

        //Find Closest Interactions to the Player 
        float shortestDistance = Mathf.Infinity;
        float distanceCheck = 0f;
        Vector3 playerPos = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
        int closestInteractionIndex = 0;

        for (int i = 0; i < sceneInteractions.Count; i++)
        {
            if (sceneInteractions[i] == null) { continue; }
            Vector3 interactionPos = new Vector3(sceneInteractions[i].anchorSelection.position.x, 0f, sceneInteractions[i].anchorSelection.position.z);
            float distance = Vector3.Distance(playerPos, interactionPos);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                closestInteractionIndex = i;
            }
        }

        //print("Closest Interaction " + closestInteractionIndex);

        if (sceneInteractions.Count > 0)
        {
            distanceCheck = sceneInteractions[closestInteractionIndex].remoteInteraction ? Director.distanceToInteractRemote : Director.distanceToInteract;

            if (sceneInteractions[closestInteractionIndex].customDistanceForInteraction != 0f)
            {
                distanceCheck = sceneInteractions[closestInteractionIndex].customDistanceForInteraction;
            }
        }

        if (shortestDistance < distanceCheck)
        {

            if (activeInteraction != sceneInteractions[closestInteractionIndex])
            {
                //print("activeInteraction "+ activeInteraction + " " + sceneInteractions[closestInteractionIndex].IsEnabled);
                if (sceneInteractions[closestInteractionIndex].IsEnabled)
                {
                    ShowSelectionMenu(true, sceneInteractions[closestInteractionIndex]);
                    activeInteraction = sceneInteractions[closestInteractionIndex];
                }
            }
        }
        else
        {
            if (activeInteraction)
            {
                activeInteraction = null;
                ShowSelectionMenu(false);
            }
        }
    }

    //PlayerManagers asks for current active interaction to resolve current interaction when player pressed interaction button
    public Interaction GetActiveInteraction(int inputSelection)
    {

        Update();

        if (activeInteraction == null) { return null; }
        if (activeInteraction.Interactions.Count == 0) { return null; }
        //print("GetActiveInteraction 1 " + activeInteraction.gameObject.name + " Length " + activeInteraction.Interactions.Count + " InputSelection " + inputSelection);

        //Input cant be bigger than active Interactions ( ie pressing R for interaction 1, which is second item at list, but list has only one item )
        if (inputSelection < activeInteraction.Interactions.Count)
        {

            if (activeInteraction.Interactions[inputSelection])
            {
                // foreach(Interaction inter in activeInteraction.Interactions) { mprint("Get Active Interaction 2 " + inter); }            
                selectedInteraction = inputSelection;
                return activeInteraction.Interactions[inputSelection];
            }
            else
            {
                //print("interactions count " + activeInteraction.Interactions.Count);
                //print("No Active Interaction");
                return null;
            }
        }
        else
        {
            print("Out of interactions " + " inputSelection " + inputSelection + " activeInteraction.Interactions.Count " + activeInteraction.Interactions.Count);
            return null;
        }
    }

    //Returns currently active interaction, ie what InteractionManager has assigned 
    public Interaction GetCurrentInteraction()
    {
        return activeInteraction.Interactions[selectedInteraction];
    }

    //Called from PlayerManager when handling types of interaction, can be called immediately or after time elapsed for collecting resource for example
    public void ActivateInteraction(bool selectionMenuStaysOn)
    {

        if (activeInteraction)
        {
            activeInteraction.Interactions[selectedInteraction].Activate();
            ShowSelectionMenu(selectionMenuStaysOn, activeInteraction);

            if (activeInteraction.Interactions[selectedInteraction].Data.interactionLife == InteractionLife.Consumable)
            {
                sceneInteractions.Remove(activeInteraction);
            }
            //Can call Find here again, if needed
        }
        else
        {
            print("No Active Interaction");
        }
    }

    public void RemoveActiveInteraction()
    {

        if (activeInteraction != null)
        {
            sceneInteractions.Remove(activeInteraction);
            activeInteraction = null;
            ShowSelectionMenu(false);
            //playerManager.InteractionFinished(false, true);
        }
    }

    public void ResetActiveInteraction()
    {
        activeInteraction = null;
    }

    //If interaction is checking for selection menu, it has to be passed to see if it is the active interaction, otherwise it should not be able to show selection menu
    public void CheckSelectionMenuVisibility(InteractionSelection checkingSelection = null)
    {
        //print("check selection menu");      
        if (activeInteraction)
        {
            //If some interaction is checking whether it should show selection menu, it needs to be the one, which is active. Otherwise other interactoin can show selection menu for current active interaction
            //If the param is null, its being checked by non interaction. PlayerManager or UIManager to see if SelectionMenu should be on
            if (checkingSelection == null)
            {
                ShowSelectionMenu(true, activeInteraction);
            }
            else
            {
                //if checking seleciton is not null, interaction is checking, then it should match active interaction
                if (activeInteraction == checkingSelection)
                {
                    ShowSelectionMenu(true, activeInteraction);
                }
            }
        }
    }

    public void FindInteractions()
    {

        //Find all objects in scene that are tagged Interaction or Character
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Interaction");
        GameObject[] characters = GameObject.FindGameObjectsWithTag("Character");

        sceneInteractions.Clear();
        foreach (GameObject obj in objs)
        {
            //print("Find Interactions " + obj.name); //+ " Selection Enabled " + obj.GetComponent<InteractionSelection>().IsEnabled);        

            if (obj.GetComponent<InteractionSelection>().IsEnabled)
            {
                sceneInteractions.Add(obj.GetComponent<InteractionSelection>());
            }

            //What Interactions are in the scene
            foreach (Interaction inter in obj.GetComponent<InteractionSelection>().Interactions)
            {
                //print(inter.gameObject.name + " " + inter.interactionType);                
            }
        }

        foreach (GameObject character in characters)
        {
            if (character.GetComponent<InteractionSelection>().IsEnabled)
            {
                sceneInteractions.Add(character.GetComponent<InteractionSelection>());
            }
        }        
    }

    public Interaction GetSceneInteraction(InteractionManager.InteractionTypes interactionType, Characters.Names? characterName = null)
    {
        Interaction sceneInteraction = null;
        FindInteractions();

        foreach (InteractionSelection selection in sceneInteractions)
        {
            foreach (Interaction selectionInteraction in selection.Interactions)
            {
                if (selectionInteraction.interactionType == interactionType)
                {

                    //Get Interaction from specific character
                    if (characterName != null)
                    {
                        Characters.Names charName = (Characters.Names)characterName;
                        if (charName == selection.gameObject.GetComponent<Character_BaseData>().Name)
                        {
                            sceneInteraction = selectionInteraction;
                        }
                    }
                    else
                    {
                        sceneInteraction = selectionInteraction;
                    }
                }
            }
        }

        //print("Returning null Interaction");
        return sceneInteraction;
    }

    public GameObject GetSceneCharacter(Characters.Names characterName)
    {
        GameObject[] interactionObjects = GameObject.FindGameObjectsWithTag("Interaction");
        foreach (GameObject interactionObj in interactionObjects)
        {
            if (interactionObj.GetComponent<Character_BaseData>())
            {
                if (interactionObj.GetComponent<Character_BaseData>().Name == characterName)
                {
                    return interactionObj;
                }
            }
        }

        return null;
    }

    public void ShowSelectionMenu(bool show, InteractionSelection interaction = null)
    {
        //print("Show Selection Menu " + show);
        selectionMenu.Setup(show, interaction);
    }

    void SetupInteractionsData()
    {

        InteractionData dataCollect = new InteractionData();
        dataCollect.animationState = State.States.Interacting;
        dataCollect.interactionType = InteractionTypes.Collect;
        dataCollect.interactionPlayerState = InteractionPlayerStates.LockedForTime;
        dataCollect.interactionLife = InteractionLife.Consumable;
        dataCollect.time = 0.2f;
        dataCollect.name = "Collect";
        dataCollect.selectionMenuStaysOn = false;
        interactionsData.Add(InteractionTypes.Collect, dataCollect);

        InteractionData dataConsume = new InteractionData();
        dataConsume.animationState = State.States.Interacting;
        dataConsume.interactionType = InteractionTypes.Consume;
        dataConsume.interactionPlayerState = InteractionPlayerStates.LockedForTime;
        dataConsume.interactionLife = InteractionLife.Consumable;
        dataConsume.time = 0.2f;
        dataConsume.name = "Use";
        dataConsume.selectionMenuStaysOn = false;
        interactionsData.Add(InteractionTypes.Consume, dataConsume);

        // InteractionData dataCollectQuick = new InteractionData();
        // dataCollectQuick.animationState = State.States.Idle;
        // dataCollectQuick.interactionType = InteractionTypes.CollectQuick;
        // dataCollectQuick.interactionPlayerState = InteractionPlayerStates.Free;
        // dataCollectQuick.interactionLife = InteractionLife.Single;
        // dataCollectQuick.time = 1.0f;
        // dataCollectQuick.name = "Collect";
        // dataCollectQuick.selectionMenuStaysOn = false;
        // interactionsData.Add(InteractionTypes.CollectQuick, dataCollectQuick);

        // InteractionData dataDialogue = new InteractionData();
        // dataDialogue.animationState = State.States.Null;
        // dataDialogue.interactionType = InteractionTypes.Dialogue;
        // dataDialogue.interactionPlayerState = InteractionPlayerStates.NotSet;
        // dataDialogue.interactionLife = InteractionLife.Active;
        // dataDialogue.time = 2.0f;
        // dataDialogue.name = "TALK";
        // dataDialogue.selectionMenuStaysOn = false;
        // interactionsData.Add(InteractionTypes.Dialogue, dataDialogue);    

        // InteractionData dataTrade = new InteractionData();
        // dataTrade.animationState = State.States.Idle;
        // dataTrade.interactionType = InteractionTypes.Trade;
        // dataTrade.interactionPlayerState = InteractionPlayerStates.Locked;
        // dataTrade.interactionLife = InteractionLife.Active;
        // dataTrade.time = 2.0f;
        // dataTrade.name = "TRADE";
        // dataTrade.selectionMenuStaysOn = false;
        // interactionsData.Add(InteractionTypes.Trade, dataTrade); 

        // InteractionData dataGiveItem = new InteractionData();
        // dataGiveItem.animationState = State.States.Idle;
        // dataGiveItem.interactionType = InteractionTypes.GiveItem;
        // dataGiveItem.interactionPlayerState = InteractionPlayerStates.Locked;
        // dataGiveItem.interactionLife = InteractionLife.Active;
        // dataGiveItem.time = 2.0f;
        // dataGiveItem.name = "GIVE";
        // dataGiveItem.selectionMenuStaysOn = false;
        // interactionsData.Add(InteractionTypes.GiveItem, dataGiveItem); 

        InteractionData dataActivate = new InteractionData();
        dataActivate.animationState = State.States.Idle;
        dataActivate.interactionType = InteractionTypes.Activate;
        dataActivate.interactionPlayerState = InteractionPlayerStates.Free;
        dataActivate.interactionLife = InteractionLife.Single;
        dataActivate.time = 0.5f;
        dataActivate.name = "ENTER";
        dataActivate.selectionMenuStaysOn = false;
        interactionsData.Add(InteractionTypes.Activate, dataActivate);

        // InteractionData dataMG = new InteractionData();
        // dataMG.animationState = State.States.Idle;
        // dataMG.interactionType = InteractionTypes.MG;
        // dataMG.interactionPlayerState = InteractionPlayerStates.Locked;
        // dataMG.interactionLife = InteractionLife.Active;
        // dataMG.time = 0.5f;
        // dataMG.name = "GATHER";
        // dataMG.selectionMenuStaysOn = false;
        // interactionsData.Add(InteractionTypes.MG, dataMG);

        // InteractionData dataCheer = new InteractionData();
        // dataCheer.animationState = State.States.Interacting;
        // dataCheer.interactionType = InteractionTypes.Cheer;
        // dataCheer.interactionPlayerState = InteractionPlayerStates.Locked;
        // dataCheer.interactionLife = InteractionLife.ActiveForTime;
        // dataCheer.time = 2.0f;
        // dataCheer.name = "CHEER UP";
        // dataCheer.selectionMenuStaysOn = true;
        // interactionsData.Add(InteractionTypes.Cheer, dataCheer);

        // InteractionData dataCauldronCraftingReady = new InteractionData();
        // dataCauldronCraftingReady.animationState = State.States.Idle;
        // dataCauldronCraftingReady.interactionType = InteractionTypes.CauldronCraftingReady;
        // dataCauldronCraftingReady.interactionPlayerState = InteractionPlayerStates.Free;
        // dataCauldronCraftingReady.interactionLife = InteractionLife.Single;
        // dataCauldronCraftingReady.time = 2.0f;
        // dataCauldronCraftingReady.name = "COOK & BREW";
        // dataCauldronCraftingReady.selectionMenuStaysOn = false;
        // interactionsData.Add(InteractionTypes.CauldronCraftingReady, dataCauldronCraftingReady);

        // InteractionData dataCauldronCraftingStart = new InteractionData();
        // dataCauldronCraftingStart.animationState = State.States.Idle;
        // dataCauldronCraftingStart.interactionType = InteractionTypes.CauldronCraftingStart;
        // dataCauldronCraftingStart.interactionPlayerState = InteractionPlayerStates.Free;
        // dataCauldronCraftingStart.interactionLife = InteractionLife.Single;
        // dataCauldronCraftingStart.time = 2.0f;
        // dataCauldronCraftingStart.name = "SELECT RECIPE";
        // dataCauldronCraftingStart.selectionMenuStaysOn = false;
        // interactionsData.Add(InteractionTypes.CauldronCraftingStart, dataCauldronCraftingStart);

        // InteractionData dataSit = new InteractionData();
        // dataSit.animationState = State.States.Sitting;
        // dataSit.interactionType = InteractionTypes.Sit;
        // dataSit.interactionPlayerState = InteractionPlayerStates.Locked;
        // dataSit.interactionLife = InteractionLife.Single;
        // dataSit.time = 0.5f;
        // dataSit.name = "SIT";
        // dataSit.selectionMenuStaysOn = true;
        // interactionsData.Add(InteractionTypes.Sit, dataSit);

        // InteractionData dataStand = new InteractionData();
        // dataStand.animationState = State.States.Idle;
        // dataStand.interactionType = InteractionTypes.Stand;
        // dataStand.interactionPlayerState = InteractionPlayerStates.Free;
        // dataStand.interactionLife = InteractionLife.Single;
        // dataStand.time = 0.5f;
        // dataStand.name = "STAND";
        // dataStand.selectionMenuStaysOn = true;
        // interactionsData.Add(InteractionTypes.Stand, dataStand);

        // InteractionData dataSleeping = new InteractionData();
        // dataSleeping.animationState = State.States.Sleeping;
        // dataSleeping.interactionType = InteractionTypes.Sleep;
        // dataSleeping.interactionPlayerState = InteractionPlayerStates.Locked;
        // dataSleeping.interactionLife = InteractionLife.Single;
        // dataSleeping.time = 0.5f;
        // dataSleeping.name = "SLEEP";
        // dataSleeping.selectionMenuStaysOn = true;
        // interactionsData.Add(InteractionTypes.Sleep, dataSleeping);

        // InteractionData dataWakeUp = new InteractionData();
        // dataWakeUp.animationState = State.States.Idle;
        // dataWakeUp.interactionType = InteractionTypes.WakeUp;
        // dataWakeUp.interactionPlayerState = InteractionPlayerStates.Free;
        // dataWakeUp.interactionLife = InteractionLife.Single;
        // dataWakeUp.time = 0.5f;
        // dataWakeUp.name = "WAKE UP";
        // dataWakeUp.selectionMenuStaysOn = false;
        // interactionsData.Add(InteractionTypes.WakeUp, dataWakeUp);

        // InteractionData dataEat = new InteractionData();
        // dataEat.animationState = State.States.Idle;
        // dataEat.interactionType = InteractionTypes.Eat;
        // dataEat.interactionPlayerState = InteractionPlayerStates.Locked;
        // dataEat.interactionLife = InteractionLife.Single;
        // dataEat.time = 0.5f;
        // dataEat.name = "EAT BREAKFAST";
        // dataEat.selectionMenuStaysOn = true;
        // interactionsData.Add(InteractionTypes.Eat, dataEat);

        // InteractionData dataHidingCharacter = new InteractionData();
        // dataHidingCharacter.animationState = State.States.Interacting;
        // dataHidingCharacter.interactionType = InteractionTypes.HidingCharacter;
        // dataHidingCharacter.interactionPlayerState = InteractionPlayerStates.Locked;
        // dataHidingCharacter.interactionLife = InteractionLife.ActiveForTime;
        // dataHidingCharacter.time = 2.5f;
        // dataHidingCharacter.name = "USE POUCH OF MANURE ALLURE";
        // dataHidingCharacter.selectionMenuStaysOn = false;
        // interactionsData.Add(InteractionTypes.HidingCharacter, dataHidingCharacter);
    }
}

public class InteractionData
{

    public InteractionManager.InteractionTypes interactionType;
    public InteractionManager.InteractionPlayerStates interactionPlayerState;
    public InteractionManager.InteractionLife interactionLife;

    //Trigger animation when interaction starts
    public State.States animationState = State.States.Null;
    public float time;
    public string name;
    public bool selectionMenuStaysOn;
}
