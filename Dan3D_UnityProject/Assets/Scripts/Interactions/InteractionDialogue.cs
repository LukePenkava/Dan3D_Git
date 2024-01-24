using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Globalization;

public class InteractionDialogue : Interaction, IDialogueInput
{
    public delegate void DialogueDelegate(Characters.Names characterName, int dialogueIndex);
    public static DialogueDelegate DialogueClosed;

    DialogueManager dialogueManager;
    State state;
    public GameObject characterParent;
    Characters.Names characterName;

    //All Dialogues data for this character. This is loaded only once at start and does not change. Only things that change are indexes
    public Dictionary<int, DialogueObject> dialoguesData;
    List<DialogueObject> dialoguesMatchingCurrentProgress = new List<DialogueObject>();
    DialogueObject activeDialogue;  //Once dialogue is set from avaialable dialouges, its stored here
    int lineIndex = 0;              //Go through dialogue lines with this index
    bool dialogueIncludesYuki = false;

    bool closeCoroutineRunning = false;

    void Start()
    {
        Init();
        dialogueManager = managers.GetComponent<DialogueManager>();
        state = characterParent.GetComponent<State>();
        characterName = characterParent.GetComponent<Character_BaseData>().Name;

        //Load All Dialogues Data for this character       
        dialoguesData = dialogueManager.LoadDialogues(characterParent.GetComponent<Character_BaseData>().Name);
    }


    //Setup Active dialouge right before interaction selection menu appears
    public override bool SetupAndCheckIfAvailable()
    {
        //Find avaialabl dialouges based on characters dialogue progression and dialogue conditions
        int availableDialougesCount = GetAvailableDialogues();

        if (availableDialougesCount > 0)
        {
            //Set Active dialogue based on available dialogues
            int dialogueIndex = Random.Range(0, dialoguesMatchingCurrentProgress.Count);
            activeDialogue = dialoguesMatchingCurrentProgress[dialogueIndex];           
        }

        bool isAvailable = availableDialougesCount > 0 ? true : false;
        return isAvailable;
    }

    //Get current amount of avaialable dialogues and store them to dialoguesMatchingCurrentProgress
    int GetAvailableDialogues()
    {
        //Load Saved data for this character
        CharacterData characterData = SaveManager.LoadCharacterData(characterName);
        int dialogueProgress = characterData.dialogueProgress;

        //Get all dialogues that Match
        dialoguesMatchingCurrentProgress = new List<DialogueObject>();
        foreach (KeyValuePair<int, DialogueObject> dialogueObj in dialoguesData)
        {
            if (dialogueObj.Value.dialogueProgress == dialogueProgress)
            {
                //Check conditions to unlock the dialogue
                bool isUnlocked = true;
                foreach (string unlockValue in dialogueObj.Value.unlockValues)
                {
                    bool result = ProcessUnlockValue(unlockValue);
                    //One fail in any of unlock values means this dialogue is locked ( all values to be true to unlock the dialouge )
                    if (result == false) { isUnlocked = false; }
                }

                if (isUnlocked)
                {
                    dialoguesMatchingCurrentProgress.Add(dialogueObj.Value);
                }
            }
        }

        //If there are no dialogues maching progress index, get some basic dialogues ( progressIndex == 0)
        if (dialoguesMatchingCurrentProgress.Count == 0)
        {
            print("No matching dialoges, use random Dialogue");
            
            foreach (KeyValuePair<int, DialogueObject> dialogueObj in dialoguesData)
            {
                if (dialogueObj.Value.dialogueProgress == 0)
                {

                    //Check conditions to unlock the dialogue
                    bool isUnlocked = true;
                    foreach (string unlockValue in dialogueObj.Value.unlockValues)
                    {
                        bool result = ProcessUnlockValue(unlockValue);
                        //One fail in any of unlock values means this dialogue is locked ( all values to be true to unlock the dialouge )
                        if (result == false) { isUnlocked = false; }
                    }

                    if (isUnlocked)
                    {
                        dialoguesMatchingCurrentProgress.Add(dialogueObj.Value);
                    }
                }
            }
        }

        return dialoguesMatchingCurrentProgress.Count;
    }

    bool ProcessUnlockValue(string unlockString)
    {
        bool unlocked = false;

        if (unlockString.Contains("tutorialIndex"))
        {
            string[] words = unlockString.Split('_');
            int tutorialIndex = int.Parse(words[1], CultureInfo.InvariantCulture);
            GameData gameData = SaveManager.LoadGameData();
            if (gameData.tutorialIndex == tutorialIndex)
            {
                unlocked = true;
            }
        }

        //QuestTaskCompleted_BasilVeggies_[Find_Veggies_DialogueClosed] => QuestTaskCompleted is unlock type, BasilVeggies is name of the quest, so the quest can be found, Find_Veggies_DialogueClosed is required completed task to unlock this dialogue
        if (unlockString.Contains("QuestTaskCompleted"))
        {
            //print("QuestTaskCompleted unlockstring " + unlockString);
            List<string> words = ProcessString(unlockString);
            QuestData qData = SaveManager.LoadQuestData();
            QuestNames.Names qName = QuestNames.StringToQuestName(words[1]);
            string taskName = words[2];

            if (qData.questObjects[qName].tasks[taskName].taskState == QuestManager.TaskState.Completed)
            {
                unlocked = true;
            }
        }

        if (unlockString.Contains("QuestCompleted"))
        {
            List<string> words = ProcessString(unlockString);
            QuestData qData = SaveManager.LoadQuestData();
            QuestNames.Names qName = QuestNames.StringToQuestName(words[1]);

            if (qData.questObjects[qName].state == QuestManager.QuestState.Completed)
            {
                unlocked = true;
            }
        }

        if (unlockString.Contains("QuestActive"))
        {
            List<string> words = ProcessString(unlockString);
            QuestData qData = SaveManager.LoadQuestData();
            QuestNames.Names qName = QuestNames.StringToQuestName(words[1]);

            if (qData.questObjects[qName].state == QuestManager.QuestState.Active)
            {
                unlocked = true;
            }
        }    

        return unlocked;
    }


    //Used by PlayerManager before the dialogue gets set, Set dialogue index and everything here
    //Used to set locked/free state of player etc
    public DialogueObject GetCurrentDialogue()
    {
        return activeDialogue;
    }

    //Spawn Yuki if he is part of the Dialogue
    // void CheckForYuki() {
    //     //If Dialogue contains Yuki, position him correctly  
    //     dialogueIncludesYuki = false;
    //     foreach(DialogueLine line in activeDialogue.dialogueLines) {           
    //         if(line.character == Dialogue.Characters.Yuki) {
    //             dialogueIncludesYuki = true;           
    //         }
    //     }

    //     if(dialogueIncludesYuki) {
    //         Vector3 danPos = GameObject.FindGameObjectWithTag("Player").transform.position;
    //         Vector3 thisPos = this.transform.position;
    //         float offsetDirection = -1f;
    //         if(thisPos.x < danPos.x) { offsetDirection = 1f; }
    //         //float xDif = danPos.x - thisPos.x;
    //         //float yukiX = thisPos.x - xDif
    //         float yukiX = danPos.x + ( 2.0f * offsetDirection);
    //         Vector3 yukiPos = new Vector3(yukiX, 0.5f, danPos.z);
    //         charactersManager.SpawnCharacterInArea(Characters.Names.Finn, null, State.States.Interacting, yukiPos);
    //         GameObject yuki = charactersManager.GetSceneCharacter(Characters.Names.Finn);
    //         yuki.GetComponent<State>().FacePosition(this.transform.position);           
    //     }
    // }

    public override void ActivateOverride()
    {
        //Set state, so that character cant start walking away or do something else        
        //state.FacePosition(gameDirector.player.transform.position); 
        //Play Idle animation for Dialogue
        // AnimSettings anim = new AnimSettings(Animations.Animation.Idle, Animations.AnimationType.Loop, false, State.States.Interacting);
        // animator.SetAnimations(anim);   
        // CheckForYuki();


        dialogueManager.CloseDialogue(false, null);
        lineIndex = 0;

        //Notify DialogueManager of this Dialogue as active Dialogue, send it this IDialogueInput. When Dialogue Manager has active dialogue, it can receive input for it and send it here 
        dialogueManager.SetActiveDialogue(this);

        //gameDirector.DialogueStarted(characterName, activeDialogue.index);
        DisplayDialogue();

        StopCoroutine("CloseCoroutine");
        if (activeDialogue.type == Dialogue.Types.Free)
        {
            StartCoroutine("CloseCoroutine");
        }
        else
        {
            //gameDirector.PauseTime(true);
        }
    }

    //Sets Dialogue based on conditions, not specficif dialogue, adjust for that if needed
    public void StartDialogueFromCode(bool playerInputEnabled, bool setDangoruAnimation)
    {
        State.Directions playerDirection = player.GetComponent<State>().Direction;

        GetComponent<State>().Direction = (playerDirection == State.Directions.Right) ? State.Directions.Left : State.Directions.Right;
        GetComponent<InteractionSelection>().IsEnabled = false;      

        dialogueManager.CloseDialogue(false, null);
        SetupAndCheckIfAvailable();
        lineIndex = 0;
        dialogueManager.SetActiveDialogue(this);
        // CheckForYuki();     
        // gameDirector.DialogueStarted(characterName, activeDialogue.index);    
        DisplayDialogue();

        GameDirector.gameState = GameDirector.GameState.LockedDialogue;
        // playerManager.InputEnabled = playerInputEnabled; 
        // playerManager.IsInteracting = true;   

        gameDirector.PauseTime(true);

        selection.IsEnabled = false;
        interactionManager.RemoveActiveInteraction();
    }

    void DisplayDialogue()
    {
        string line = activeDialogue.dialogueLines[lineIndex].line;
        dialogueManager.DisplayText(activeDialogue, lineIndex);
    }

    void CloseDialogue()
    {

        selection.IsEnabled = true;
        //End of Dialogue
        GameDirector.gameState = GameDirector.GameState.World;

        dialogueManager.CloseDialogue(true, activeDialogue);
        playerManager.InteractionFinished(true, false);

        lineIndex = 0;

        //gameDirector.DialogueClosed(characterName, activeDialogue.index);   
        if (DialogueClosed != null)
        {
            DialogueClosed(characterName, activeDialogue.index);
        }

        // if(dialogueIncludesYuki) {
        //     dialogueIncludesYuki = false;            
        //     SpriteAnimator yukiAnimator = charactersManager.GetSceneCharacter(Characters.Names.Finn).GetComponent<SpriteAnimator>();
        //     AnimSettings yukiAnim = new AnimSettings(Animations.Animation.Idle, Animations.AnimationType.Loop, true, State.States.Idle);
        //     yukiAnimator.SetAnimations(yukiAnim);
        // }

        interactionManager.FindInteractions();        
    }

    IEnumerator CloseCoroutine()
    {
        closeCoroutineRunning = true;
        float timer = 1.2f + activeDialogue.dialogueLines[lineIndex].line.Length * 0.05f;
        yield return new WaitForSeconds(timer);

        CloseDialogue();
        closeCoroutineRunning = false;
    }



    #region Interfaces

    //Player presses button, progress Dialogue, comes from DialogueManager, which knows about this as active Dialogue
    public void DialogueInput()
    {
        //Check if Dialogue is currently being typed, if so and player pressed button, finish the typing        
        if (dialogueManager.IsTyping)
        {
            dialogueManager.FinishTyping();
        }
        else
        {
            lineIndex++;
           
            if (lineIndex >= activeDialogue.dialogueLines.Count)
            {
                CloseDialogue();
            }
            else
            {
                DisplayDialogue();
            }
        }
    }

    #endregion


    List<string> ProcessString(string text)
    {

        List<string> words = new List<string>();
        string curWord = "";
        bool isBrackets = false;

        foreach (char c in text)
        {
            if (c == '[') { isBrackets = true; continue; }
            if (c == ']')
            {
                isBrackets = false;
                words.Add(curWord);
                curWord = "";
                continue;
            }

            if (c == '_' && isBrackets == false)
            {
                words.Add(curWord);
                curWord = "";
            }
            else
            {
                curWord += c;
            }
        }

        if (curWord.Length > 0)
        {
            words.Add(curWord);
            curWord = "";
        }

        return words;
    }
}
