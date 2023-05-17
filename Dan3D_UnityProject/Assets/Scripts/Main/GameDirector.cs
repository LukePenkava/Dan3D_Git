using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameDirector : MonoBehaviour
{    
    public delegate void TimeDelegate(bool isPaused, bool mainPause);
    public static TimeDelegate TimePausedEvent;  

    AreaManager areaManager; 
    
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
    public GameObject zima;

    void OnEnable() {
        Area.AreaLoaded += AreaLoaded;
    }

    void OnDisable() {
        Area.AreaLoaded -= AreaLoaded;
    }

    // Start is called before the first frame update
    public void Init()
    {
        areaManager = GetComponent<AreaManager>();
        areaManager.LoadArea(Areas.Area_1);

        //PerformDynamicRes scaler = PerformDynamicRes.
        //DynamicResolutionHandler.SetDynamicResScaler()
        //print(ItemsDirectory.ItemsData[Items.ItemName.Wood].description);

        //Setup Dans starting items
        List<Items.ItemName> itemList = new List<Items.ItemName>();
        itemList.Add(Items.ItemName.Wood);               
        //itemList.Add(Items.ItemName.Stardew);               
        player.GetComponent<Character_BaseData>().AddItems(itemList);        
    }

    void AreaLoaded(Area areaScript) {
        //Position Player and Zima

        //Find Spawn Pos that corresponds to previous area
        Vector3 pos = areaScript.spawnPositins[0].gameObject.transform.position; //Use first spawnPos as default one, if no match was found
        foreach(AreaSpawnPos spawnPos in areaScript.spawnPositins) {            
            if(spawnPos.prevArea == areaManager.PrevArea) {
                pos = spawnPos.gameObject.transform.position;
            }
        }

        player.transform.position = new Vector3(pos.x, pos.y, pos.z);
        zima.transform.position = new Vector3(pos.x, pos.y, pos.z - 1f);
    }

    //Event notifying TimeManager and CharacterStatus for curse to pause. For example when its tutorial, dialogue or trading
    public void PauseTime(bool isPaused, bool mainPause = false) {        
        if(TimePausedEvent != null) {           
            TimePausedEvent(isPaused, mainPause);
        }
    }
}
