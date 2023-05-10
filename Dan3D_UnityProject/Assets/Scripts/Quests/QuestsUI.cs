using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestsUI : MonoBehaviour
{
    QuestManager questManager;
    QuestData qData;
    UIManager uiManager;
    Character_BaseData playerBaseData;
    public UISwitch switchButtons;

    public GameObject questPrefab;  
    public GameObject taskPrefab; 
    
    public RectTransform uiParent;
    public UIScrollArea scrollArea;
    public Transform questsParent; 
    public Transform tasksParent;   

    float menuWidth = 1044;
    float menuHeight = 537;
    float questHeight = 60f;
    float questWidth = 0f;
    float gapBetweenItems = 10f;
    float scrollAreaPadding = 20f;    
    float pageHeight = 460f;

    List<GameObject> questObjs = new List<GameObject>();

    public Text quest_title;
    public Text quest_description; 

    

    public void Init(QuestManager.QuestState qState = QuestManager.QuestState.Active)
    {
        uiManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<UIManager>();
        playerBaseData = GameObject.FindGameObjectWithTag("Player").GetComponent<Character_BaseData>();
        questManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<QuestManager>();
        qData = SaveManager.LoadQuestData();
        

        foreach(GameObject questObj in questObjs) {
            Destroy(questObj);
        }
        questObjs.Clear();

        foreach(Transform temp in tasksParent) {
            Destroy(temp.gameObject);
        }    
        quest_title.text = "";
        quest_description.text = "";    
        
        menuWidth = 800;
        menuHeight = 700;
        questHeight = 80f;
        questWidth = 780f;
        gapBetweenItems = 10f;
        scrollAreaPadding = 20f;    
        pageHeight = 460f;      
       
        uiParent.sizeDelta = new Vector2(menuWidth, menuHeight);
        Vector2 scrollAreaSize = scrollArea.SetGaps(gapBetweenItems); 

        //Calculate bottom limit based on how many recipes there is
        int recipesInSelectionBox = Mathf.FloorToInt(menuHeight / questHeight);
        float bottomLimit = (qData.questObjects.Count - 1) * questHeight;  //Start position for first box is 0, next one is 100. So 4th start position is 300. Because of that its -1
      
        float height = pageHeight - gapBetweenItems;        
        float totalLength = (Mathf.CeilToInt((float)2 / (float)1) * questHeight) - menuHeight + scrollAreaPadding;
        scrollArea.Init(totalLength, 0, questHeight, pageHeight, gapBetweenItems);

       

        float startPosY = questHeight/-2f;
        int index = 0;
        foreach(KeyValuePair<QuestNames.Names, QuestObject> quest in qData.questObjects) {
            //print(quest.Value.name + " " + quest.Value.state);

            if(quest.Value.state == qState) {             
                float posY = startPosY - (index * questHeight) - (index * gapBetweenItems);
                float width = menuWidth - 2*gapBetweenItems; //Edges on left and right to be same as at the top and bototm
                CreateQuestsUI(quest.Value, posY, width, questHeight);

                index++;
            }
        }

        //Select first Quest by default
        if(questObjs.Count > 0) {
            questObjs[0].GetComponent<QuestObjectUI>().SetFocus();
        }
    }


    void CreateQuestsUI(QuestObject quest, float posY, float MenuWidth, float RecipeHeight) {   

        GameObject newObj = (GameObject)Instantiate(questPrefab, Vector3.zero, Quaternion.identity);
        newObj.transform.SetParent(questsParent);
        newObj.transform.localPosition = new Vector2(0f, posY);
        newObj.transform.localScale = Vector3.one;
        newObj.GetComponent<QuestObjectUI>().Init(quest, this, scrollArea, questWidth, RecipeHeight, playerBaseData);

        questObjs.Add(newObj);
    }

     public void Focused(QuestObject quest) {

        quest_title.text = quest.title;
        quest_description.text = quest.description;

        //Create Tasks
        float posStep = 70f; 
        float gap = 15f;

        foreach(Transform temp in tasksParent) {
            Destroy(temp.gameObject);
        }

        int taskCount = 0;   
        foreach(var task in quest.tasks) {     

            GameObject taskObj = Instantiate(taskPrefab, Vector3.zero, Quaternion.identity);
            taskObj.transform.SetParent(tasksParent);
            taskObj.transform.localScale = Vector3.one;
            taskObj.transform.localPosition = new Vector2(0f, posStep * taskCount * -1f - (gap *taskCount));
            taskObj.GetComponent<TaskObjectUI>().Init(task.Value);

            taskCount++;
        }
    }

    public void Close() {
        Reset();
        uiParent.gameObject.SetActive(false);
    }

    public void Reset() {
        
        foreach(GameObject questObj in questObjs) {
            Destroy(questObj);
        }
        questObjs.Clear();

        uiManager.DeselectElement();
    }

    //Preselect Active Quests, this is as if player select button, goes to switch, then goes here to SelectQuestType
    public void SelectQuests() {
        switchButtons.Switch(0);
    }

    public void SelectQuestType(int index) {       

        switch(index) {
            case 0:
                Init();
                break;
            case 1:
                Init(QuestManager.QuestState.Completed);
                break;
            default:
                break;
        }      
    }
}
