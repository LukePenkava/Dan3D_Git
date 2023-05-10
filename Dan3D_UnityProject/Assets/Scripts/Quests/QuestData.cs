using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    //Holds all data for all quests 
    public Dictionary<QuestNames.Names, QuestObject> questObjects = new Dictionary<QuestNames.Names, QuestObject>();

    //Want to get value of taskValue of specific quest. taskNames and value contain multiple keys ("Find_DiloagueClosed"), so it has to be searched for 
    public string GetTaskValue(QuestNames.Names questName, string taskName, string taskValueKey) {
        
        //Go through each task and check if its name contains taskName
        foreach(KeyValuePair<string, QuestTask> task in questObjects[questName].tasks) {
           
            if(task.Value.name.Contains(taskName)) {
                //Go through each taskValue in task and check if it contains the taskValueKey
                foreach(KeyValuePair<string, string> taskValue in task.Value.taskValues) {
                    //If it does, return its value                   
                    if(taskValue.Key.Contains(taskValueKey)) {
                        return taskValue.Value;
                    }
                }
            }
        }

        //Did not find the task value
        return "null";
    }
}
