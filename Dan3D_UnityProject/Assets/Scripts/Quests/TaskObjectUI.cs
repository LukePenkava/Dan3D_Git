using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskObjectUI : MonoBehaviour
{
    public GameObject checkmark;
    public Text taskName;
    public Text taskValues;
    public Image bg;

    Color normal = Color.grey;
    Color completed = Color.green;
    public Color bgColor_Hidden;
    public Color bgColor_Active;
    public Color bgColor_Completed;
    public Color textColor_Hidden;
    public Color textColor_Active;
    public Color textColor_Completed;

    public void Init(QuestTask task) {

        bool taskCompleted = task.taskState == QuestManager.TaskState.Completed;

        taskName.text = task.description;       
        checkmark.gameObject.SetActive(taskCompleted);
        //Colors
        switch(task.taskState) {
            case QuestManager.TaskState.Hidden:
                bg.color = bgColor_Hidden;
                taskName.color = textColor_Hidden;
                taskValues.color = textColor_Hidden;
                break; 
            case QuestManager.TaskState.Active:
                bg.color = bgColor_Active;
                taskName.color = textColor_Active;
                taskValues.color = textColor_Active;
                break; 
            case QuestManager.TaskState.Completed:
                bg.color = bgColor_Completed;
                taskName.color = textColor_Completed;
                taskValues.color = textColor_Completed;
                break;  
        }

        //Task Values is for example 1/4 veggies found. Display this only when there is more than one taskValue, if its only one taskValue, like go talk to someone or smt, dont display Values
        if(task.taskValues.Count > 1) {
            taskValues.gameObject.SetActive(true);

            int totalCount = task.taskValues.Count;
            int completedCount = 0;
            foreach(KeyValuePair<string, string> taskValue in task.taskValues) {
                if(taskValue.Value == task.completedValue) {
                    completedCount++;
                }
            }
            taskValues.text = completedCount.ToString() + "/" + totalCount.ToString();

        } else {
            taskValues.gameObject.SetActive(false);
        }
    }
}
