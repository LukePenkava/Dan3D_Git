using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameDirector : MonoBehaviour
{    
    public delegate void TimeDelegate(bool isPaused, bool mainPause);
    public static TimeDelegate TimePausedEvent;    
    
    public enum GameState {
        World,
        LockedDialogue,
        UI,
        MG,
        Crafting
    }
    public static GameState gameState;

    public enum DayPhase {
        Day,
        Night,
        Both
    } 
    public static DayPhase dayPhase;

    //Variables
    public GameObject player;



    // Start is called before the first frame update
    public void Init()
    {
        //PerformDynamicRes scaler = PerformDynamicRes.
        //DynamicResolutionHandler.SetDynamicResScaler()
        //print(ItemsDirectory.ItemsData[Items.ItemName.Wood].description);

        //Setup Dans starting items
        List<Items.ItemName> itemList = new List<Items.ItemName>();
        itemList.Add(Items.ItemName.Wood);               
        //itemList.Add(Items.ItemName.Stardew);               
        player.GetComponent<Character_BaseData>().AddItems(itemList);
    }

    //Event notifying TimeManager and CharacterStatus for curse to pause. For example when its tutorial, dialogue or trading
    public void PauseTime(bool isPaused, bool mainPause = false) {        
        if(TimePausedEvent != null) {           
            TimePausedEvent(isPaused, mainPause);
        }
    }
}
