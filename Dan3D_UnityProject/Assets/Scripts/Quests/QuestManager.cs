using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.IO;
using System.Globalization;


public class QuestManager : MonoBehaviour
{
    /*  DOCUMENTATION ON QUESTS
        -Quests are defined in Xml, they are loaded and stored only at first launch, all quest data. After that, quests xml never loads again and all the quest data are retrieved from saved data in QuestData
        -TaskNames need keywords to use functions to complete the tasks
        -Progres is shonw in Inventory with QuestUI script
    */


    UIManager uiManager;

    QuestData questsData;
    public QuestData QuestsData {
        get { return questsData; }
    } 

    public enum QuestState {
        Hidden,     //Player did not get/find this quest yet
        Active,
        Completed
    };

    public enum TaskState {
        Hidden,
        Active,
        Completed
    }

    
    public void Init()
    {
        uiManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<UIManager>();
        QuestData loadedQuestsData = SaveManager.LoadQuestData();

        //There are no data, first play, setup quest data, based on Xml. Ie get all the quest objects and store them into saved data, then dont load xml again. This happens only once until game is completely reseted and its all saves
        if(loadedQuestsData.questObjects.Count == 0) {
            LoadQuestsXml();
            SaveManager.SaveQuestData(questsData);
        } else {
            //use loaded quest data            
            questsData = loadedQuestsData;
        }        
    }

    void OnEnable() {
        // InteractionDialogue.DialogueClosed += DialogueClosed;
        // CraftingManager.ItemCraftedEvent += ItemCrafted;
        // //Inventory.GiveItemEvent += ItemGiven;
    }

    void OnDisable() {
    //     InteractionDialogue.DialogueClosed -= DialogueClosed;
    //     CraftingManager.ItemCraftedEvent -= ItemCrafted;
    //    // Inventory.GiveItemEvent -= ItemGiven;
    }

    //Load all quests data, loaded only once at first play not again 
    void LoadQuestsXml() {

        questsData = new QuestData();
        TextAsset file = (TextAsset)Resources.Load("XmlData/Quests/QuestData");

        if(file != null) {
            XmlSerializer serializer = new XmlSerializer(typeof(XmlQuests));
            XmlQuests quests = serializer.Deserialize(new StringReader(file.text)) as XmlQuests;

            foreach(XmlQuest quest in quests.questsList) {

                QuestObject newQuestObject = new QuestObject();

                newQuestObject.name = QuestNames.StringToQuestName(quest.name);
                newQuestObject.title = quest.title;
                newQuestObject.description = quest.description;
                newQuestObject.state = QuestState.Hidden;
                if(newQuestObject.name == QuestNames.Names.LostSundial) { newQuestObject.state = QuestState.Active; } //First default quest, on by default, exception                

                Dictionary<string, QuestTask> tasks = new Dictionary<string, QuestTask>();
                if(quest.tasks.Length > 0) {
                    //Go through every task
                    foreach(XmlTask task in quest.tasks) {
                        //For each task, create QuestTask object and set it up
                        QuestTask newQuestTask = new QuestTask();
                        newQuestTask.index = int.Parse(task.index, CultureInfo.InvariantCulture);
                        newQuestTask.name = task.name;
                        newQuestTask.description = task.description;
                        newQuestTask.narratorProgress = task.narratorProgress;
                        newQuestTask.narratorCompleted = task.narratorCompleted;
                        newQuestTask.completedValue = task.completedValue;                       
                        newQuestTask.taskState = TaskState.Hidden;

                        //Create Dictionary for task values
                        Dictionary<string, string> taskValues = new Dictionary<string, string>();
                        if(task.taskValues.Length > 0) {
                            foreach(XmlTaskValue taskValue in task.taskValues) {
                                taskValues.Add(taskValue.Key, taskValue.Value);
                            }
                        }
                        newQuestTask.taskValues = taskValues;

                        tasks.Add(newQuestTask.name, newQuestTask);
                    }
                }     
                newQuestObject.tasks = tasks;

                //Object completed          

                questsData.questObjects.Add(newQuestObject.name, newQuestObject);
            }            
        }
        else {
            Debug.Log("Items Xml File is Null");
        }
    }

   // public void SaveQuestState(QuestNames.Names questName, QuestManager.QuestState questState,

    #region Ways to Complete task

    //One of the ways how to progress/complete task is by talking to characters
    //Whenever any dialogue closed, check if any quest and its task is looking for this conversation. Ie dialogue with specific character and specific dialogue index
    //TaskName must include keyword "DialogueClosed" to be checked
    //Check if it is match by Value of taskValue, ie "Carrot_1" is looking for Carrot character, dialogueIndex == 1
    public void DialogueClosed(Characters.Names characterName, int dialogueIndex) {
        QuestData qData = SaveManager.LoadQuestData();

        foreach(KeyValuePair<QuestNames.Names, QuestObject> quest in qData.questObjects) {

            if(quest.Value.state == QuestState.Active) {                
                foreach(KeyValuePair<string, QuestTask> task in quest.Value.tasks) {
                    if(task.Value.name.Contains("DialogueClosed")) {
                        
                        foreach(KeyValuePair<string, string> taskValue in task.Value.taskValues) {
                            
                            string taskValueString = taskValue.Key;
                            //string is in this format Carrot_1, separate string by _ => first index is character name, second is dialogueIndex
                            string[] words = taskValueString.Split('_');
                            Characters.Names charName = (Characters.Names)System.Enum.Parse(typeof(Characters.Names), words[0], true);  
                            int index = int.Parse(words[1], CultureInfo.InvariantCulture); 

                            //Set the task value to correct value, this marks the task completed
                            if(characterName == charName && index == dialogueIndex) {
                                qData.questObjects[quest.Value.name].tasks[task.Value.name].taskValues[taskValue.Key] = task.Value.completedValue;                                 
                                CheckIfTaskAndQuestCompleted(qData, quest.Value.name, task.Value.name);    
                                return;                          
                            }
                        }
                    }
                }
            }
        }
    }

    //Check if correct item was crafted that is part of the quest
    void ItemCrafted(Items.ItemName itemName, Items.ItemTags[] itemTags) {
        QuestData qData = SaveManager.LoadQuestData();

        foreach(KeyValuePair<QuestNames.Names, QuestObject> quest in qData.questObjects) {

            if(quest.Value.state == QuestState.Active) {     
                foreach(KeyValuePair<string, QuestTask> task in quest.Value.tasks) {
                    if(task.Value.name.Contains("Craft")) {

                        foreach(KeyValuePair<string, string> taskValue in task.Value.taskValues) {
                            
                            string taskValueString = taskValue.Key;
                            //Can check for crafting specific item or for tag item, like tag_Food => check if food was crafted
                            string[] words = taskValueString.Split('_');
                            if(words[0] == "Tags") {
                                Items.ItemTags tag = (Items.ItemTags)System.Enum.Parse(typeof(Items.ItemTags), words[1], true);

                                foreach(Items.ItemTags itemTag in itemTags){ 
                                    if(tag == itemTag) {
                                        qData.questObjects[quest.Value.name].tasks[task.Value.name].taskValues[taskValue.Key] = task.Value.completedValue;                                 
                                        CheckIfTaskAndQuestCompleted(qData, quest.Value.name, task.Value.name);
                                        return;
                                    }                            
                                }
                            }

                            //See if specific item was crafted
                            if(words[0] == "Name") {
                                Items.ItemName craftItemName = (Items.ItemName)System.Enum.Parse(typeof(Items.ItemName), words[1], true);
                                if(craftItemName == itemName) {
                                    qData.questObjects[quest.Value.name].tasks[task.Value.name].taskValues[taskValue.Key] = task.Value.completedValue;                                 
                                    CheckIfTaskAndQuestCompleted(qData, quest.Value.name, task.Value.name);
                                    return;
                                } 
                            }
                        }

                    }
                }
            }
        }
    }

    //Comes directly from Inventory ( not event ), so progress is saved before everything else
    //Check if correct item was crafted that is part of the quest
    public void GiveItemEvent(Characters.Names charName, Items.ItemName itemName, Items.ItemTags[] itemTags) {
        QuestData qData = SaveManager.LoadQuestData();

        foreach(KeyValuePair<QuestNames.Names, QuestObject> quest in qData.questObjects) {

            if(quest.Value.state == QuestState.Active) {     
                foreach(KeyValuePair<string, QuestTask> task in quest.Value.tasks) {
                    if(task.Value.name.Contains("Give") && task.Value.name.Contains(charName.ToString())) {

                        foreach(KeyValuePair<string, string> taskValue in task.Value.taskValues) {
                            
                            string taskValueString = taskValue.Key;
                            //Can check for crafting specific item or for tag item, like tag_Food => check if food was crafted
                            string[] words = taskValueString.Split('_');
                            if(words[0] == "Tags") {
                                Items.ItemTags tag = (Items.ItemTags)System.Enum.Parse(typeof(Items.ItemTags), words[1], true);

                                foreach(Items.ItemTags itemTag in itemTags){ 
                                    if(tag == itemTag) {
                                        qData.questObjects[quest.Value.name].tasks[task.Value.name].taskValues[taskValue.Key] = task.Value.completedValue;                                 
                                        CheckIfTaskAndQuestCompleted(qData, quest.Value.name, task.Value.name);
                                        return;
                                    }                            
                                }
                            }

                            if(words[0] == "Name") {
                                Items.ItemName giveItemName = (Items.ItemName)System.Enum.Parse(typeof(Items.ItemName), words[1], true);
                                
                                if(giveItemName == itemName) {
                                    qData.questObjects[quest.Value.name].tasks[task.Value.name].taskValues[taskValue.Key] = task.Value.completedValue;                                 
                                    CheckIfTaskAndQuestCompleted(qData, quest.Value.name, task.Value.name);
                                    return;
                                }                           
                            
                            }
                        }

                    }
                }
            }
        }
    }

    

    #endregion

    //Universal function for all functions completing/progressing task. 
    //This function check progress of very task/quest and marks it completed, no matter how it progressed.
    void CheckIfTaskAndQuestCompleted(QuestData qData, QuestNames.Names qName, string taskName) {     

        //print("Check Quest " + qName + " taskName " + taskName) ;  

        //Check all taskValues in task. If all have completed value, then the task is completed. If only one does not equal completedValue then the task is not completed
        bool taskIsCompleted = true;
        int completedValues = 0;
        int totalValues = 0;
        foreach(KeyValuePair<string, string> taskValue in qData.questObjects[qName].tasks[taskName].taskValues) {
            if(taskValue.Value != qData.questObjects[qName].tasks[taskName].completedValue) {
                taskIsCompleted = false;
            } else {
                completedValues++;
            }
            totalValues++;
        }

        if(taskIsCompleted) {
            qData.questObjects[qName].tasks[taskName].taskState = QuestManager.TaskState.Completed;    
            qData = SetActiveTask(qData, qName);
        }

        //Show Narrator for task, there is different narrator text for progressing through task and completing it. It can also be completely ignored and no task displayed
        if(taskIsCompleted) {
            string narratorText = qData.questObjects[qName].tasks[taskName].narratorCompleted;
            //Show narrator only if there is some text to show, can be empty
            if(narratorText.Length > 0) {
                //uiManager.ShowNarrator(narratorText, 3f);
            }

        } else {
            string narratorText = qData.questObjects[qName].tasks[taskName].narratorProgress;         
            if(narratorText.Length > 0) {
                narratorText += " " + completedValues.ToString() + "/" + totalValues.ToString();
                //uiManager.ShowNarrator(narratorText, 3f);
            }
        }               


        //Check if all tasks are completed. If so, quest is completed
        bool questCompleted = true;
        foreach(KeyValuePair<string, QuestTask> task in qData.questObjects[qName].tasks) {
            if(task.Value.taskState == QuestManager.TaskState.Hidden || task.Value.taskState == QuestManager.TaskState.Active) {
                questCompleted = false;
            }
        }
        if(questCompleted) {
            qData.questObjects[qName].state = QuestState.Completed;
        }      

        //print("taskIsCompleted " + taskIsCompleted + " questCompleted " + questCompleted);  

        SaveManager.SaveQuestData(qData);
    }

    public void SetQuestState(QuestNames.Names qName, QuestState qState) {
        QuestData qData = SaveManager.LoadQuestData();
        qData.questObjects[qName].state = qState;  
        qData = SetActiveTask(qData, qName);
        SaveManager.SaveQuestData(qData);
    }

    QuestData SetActiveTask(QuestData qData, QuestNames.Names qName) {    

        //Go through all tasks and set active task
        //since tasks are dict, its not order and can be unorderd, So go through tasks indexes starting from 1 to last one
        //Set first hiddent task to be Active
        for(int i = 1; i <= qData.questObjects[qName].tasks.Count; i++) {
            foreach(var task in qData.questObjects[qName].tasks) {
                if(task.Value.index == i) {
                    if(task.Value.taskState == TaskState.Hidden) {
                        task.Value.taskState = TaskState.Active;
                        print("Active Task " + task.Value.name);
                        return qData;
                    }
                }
            }            
        }

        //Dit not set any task Active
        return qData;
    }
    
}
