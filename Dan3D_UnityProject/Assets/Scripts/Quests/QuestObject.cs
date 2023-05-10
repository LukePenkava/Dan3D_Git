using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestObject {

    //Values form Xml
    public QuestNames.Names name;
    public string title;
    public string description;   
    //One or more tasks for this quest
    public Dictionary<string, QuestTask> tasks;   
    public QuestManager.QuestState state;      
}

[System.Serializable]
public class QuestTask {

    public int index;
    public string name;
    public string description;
    public QuestManager.TaskState taskState;
    public string narratorProgress;
    public string narratorCompleted;
    public string completedValue;
    //Actual things player does to complete tasks => quest
    public Dictionary<string, string> taskValues; 

}
