using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text;
using System.Linq;
using System.Globalization;

public class DialogueManager : MonoBehaviour
{
    IDialogueInput activeDialogue;
    PlayerManager playerManager;
    InteractionManager interactionManager;
    UIManager uiManager;

    public DialogueBubble mainBubble;

    public enum DialogueCharacters
    {
        Dangoru,
        Character,
        Zima
    }

    int activeDialogueProgressIndex = 0;

    bool isTyping = false;
    public bool IsTyping
    {
        get { return isTyping; }
        set { isTyping = value; }
    }

    bool useNarrator = false;

    public void Init()
    {
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
        interactionManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<InteractionManager>();
        uiManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<UIManager>();

        CloseDialogue(false, null);
    }

    public void SetActiveDialogue(IDialogueInput dialogue)
    {
        activeDialogue = dialogue;
    }

    //Progress Dialogue. Call InteractionDialogue, which triggered dialogued
    public void PlayerInput()
    {
        if (activeDialogue != null)
        {
            //Goes to Interaction Dialogue
            activeDialogue.DialogueInput();
        }
        else
        {
            print("No Active Dialogue");
        }
    }

    //Called from Dialogue Interaction, to set dialogue from code or not to progerss dialouge set ProgressIndex to 0  
    public void DisplayText(DialogueObject activeDialogue, int lineIndex)
    {
        string text = activeDialogue.dialogueLines[lineIndex].line;
        Dialogue.Types dialogueType = activeDialogue.type;
        activeDialogueProgressIndex = activeDialogue.dialogueProgress;

        mainBubble.gameObject.SetActive(true);
        mainBubble.SetText(text, dialogueType);
        isTyping = true;
    }

    public void FinishTyping()
    {
        mainBubble.Finish();
    }

    public void CloseDialogue(bool resetInteraction, DialogueObject ActiveDialogue)
    {
        //InteractionManager needs to get activeInteraction set to null, so the selection menu does not show up for split second
        if (resetInteraction)
        {
            interactionManager.ResetActiveInteraction();
        }

        //After closing dialogue increase progress dialogue index, if dialogue progess index was other than 0 ( ie it was dialogue based on progress Index )
        if (activeDialogue != null)
        {
            //Dont progress if its random dialogue
            if (ActiveDialogue.dialogueProgress != 0)
            {
                Character_BaseData character = activeDialogue.gameObject.GetComponent<InteractionDialogue>().characterParent.GetComponent<Character_BaseData>();
                CharacterData characterData = SaveManager.LoadCharacterData(character.Name);
                characterData.dialogueProgress++;
                print("Saving Dialogue Progress: " + characterData.dialogueProgress);
                SaveManager.SaveCharacterData(character.Name, characterData);
            }
        }

        activeDialogue = null;
        mainBubble.gameObject.SetActive(false);
    }

    //Each Character loads his own Data in his start function. These data are loaded only once and he has always access to them. These data dont change
    public Dictionary<int, DialogueObject> LoadDialogues(Characters.Names characterName)
    {
        TextAsset file = (TextAsset)Resources.Load("XmlData/Dialogues/" + characterName.ToString() + "/" + characterName.ToString() + "_Dialogue"); // can add suffix if more Xmls will be needed
        Dictionary<int, DialogueObject> dialoguesDict = new Dictionary<int, DialogueObject>();

        if (file != null)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlDialogues));
            XmlDialogues dialogueObjects = serializer.Deserialize(new StringReader(file.text)) as XmlDialogues;

            foreach (XmlDialogue dialogue in dialogueObjects.DialogueList)
            {

                //Create Object and assign it Data from Xml
                DialogueObject newDialogue = new DialogueObject();

                newDialogue.index = int.Parse(dialogue.Index, CultureInfo.InvariantCulture);
                newDialogue.dialogueProgress = int.Parse(dialogue.DialogueProgress, CultureInfo.InvariantCulture);
                newDialogue.unlockValues = dialogue.UnlockValues;
                newDialogue.type = GetType(dialogue.Type);

                List<DialogueLine> dialogueLines = new List<DialogueLine>();
                foreach (XmlDialogueLine dialogueLine in dialogue.LinesArray)
                {
                    DialogueLine newLine = new DialogueLine();
                    newLine.index = int.Parse(dialogueLine.Index, CultureInfo.InvariantCulture);
                    newLine.character = GetCharacter(dialogueLine.Character);
                    newLine.line = dialogueLine.Line;
                    newLine.vo = dialogueLine.VO;
                    newLine.type = dialogueLine.Type;

                    dialogueLines.Add(newLine);
                }
                newDialogue.dialogueLines = dialogueLines;
                //Object Completed    

                //Add it to Dictionary
                dialoguesDict.Add(newDialogue.index, newDialogue);
            }

            return dialoguesDict;
        }
        else
        {
            Debug.Log("Items Xml File is Null");
            return null;
        }
    }


    Dialogue.Characters GetCharacter(string val)
    {
        switch (val)
        {
            case "Dangoru":
                return Dialogue.Characters.Dangoru;
            case "This":
                return Dialogue.Characters.Character;
            case "Zima":
                return Dialogue.Characters.Zima;
            case "Narrator":
                return Dialogue.Characters.Narrator;
            default:
                print("Type does not exist");
                return Dialogue.Characters.NotSet;
        }
    }

    Dialogue.Types GetType(string val)
    {
        switch (val)
        {
            case "Free":
                return Dialogue.Types.Free;
            case "Locked":
                return Dialogue.Types.Locked;
            default:
                print("Type does not exist");
                return Dialogue.Types.NotSet;
        }
    }

}

public class DialogueObject
{

    public int index;
    public int dialogueProgress = 0;
    public string[] unlockValues;
    public Dialogue.Types type;
    public List<DialogueLine> dialogueLines;

    public DialogueObject() { }

    public DialogueObject(Dialogue.Types DialogueType, params DialogueLine[] lines)
    {
        index = 0;
        dialogueProgress = 0;
        unlockValues = null;
        type = DialogueType;
        dialogueLines = lines.ToList();
    }
}

public class DialogueLine
{
    public int index;
    public Dialogue.Characters character;
    public string line;
    public string vo;
    public string type;

    public DialogueLine() { }

    public DialogueLine(int Index, Dialogue.Characters Character, string Line, string VO, string Type)
    {
        index = Index;
        character = Character;
        line = Line;
        vo = VO;
        type = Type;
    }
}


