using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameDirector : MonoBehaviour
{    
    public delegate void TimeDelegate(bool isPaused, bool mainPause);
    public static TimeDelegate TimePausedEvent;  

    AreaManager areaManager; 
    UIManager uiManager;
    
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
    public Areas firstAreaToLoad = Areas.Area_1;
    public GameObject player;
    public GameObject zima;

    void OnEnable() {
        AreaManager.AreaLoaded += AreaLoaded;
    }

    void OnDisable() {
        AreaManager.AreaLoaded -= AreaLoaded;
    }

    // Start is called before the first frame update
    public void Init()
    {
        GameDirector.gameState = GameDirector.GameState.World;

        uiManager = GetComponent<UIManager>();
        areaManager = GetComponent<AreaManager>();

        uiManager.loadOverlay.SetActive(true);
        areaManager.LoadArea(firstAreaToLoad);

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
        // Vector3 pos = areaScript.spawnPositins[0].gameObject.transform.position; //Use first spawnPos as default one, if no match was found
        // foreach(AreaSpawnPos spawnPos in areaScript.spawnPositins) {            
        //     if(spawnPos.prevArea == areaManager.PrevArea) {
        //         pos = spawnPos.gameObject.transform.position;
        //     }
        // }

        Vector3 pos = Vector3.zero;
        bool setFirst = false;
        foreach(Transform spawnPos in areaScript.spawnPositions) {

            if(setFirst == false) {
                setFirst = true;
                pos = spawnPos.position;
            }

            AreaSpawnPos spawnPosScript = spawnPos.gameObject.GetComponent<AreaSpawnPos>();

            if(spawnPosScript.prevArea == areaManager.PrevArea) {
                pos = spawnPos.position;
            }
        }        

        if(areaScript.location == Locations.Forest) {
            zima.SetActive(true);
            zima.transform.position = new Vector3(pos.x, pos.y, pos.z - 1f);
            zima.GetComponent<Zima>().SetHome(false);
        }
        else {
            if(areaScript.area == Areas.Home_1 || areaScript.area == Areas.Home_3) {
                zima.SetActive(false);
            } 
            else if(areaScript.area == Areas.Home_2) {
                zima.SetActive(true);
                zima.transform.position = new Vector3(-0.45f, 0.21f, -2.67f);
                zima.transform.localEulerAngles = new Vector3(0,0,0);
                zima.GetComponent<Zima>().SetHome(true);
            }
        }

        
        //player.transform.position =  new Vector3(pos.x, pos.y, pos.z);     
        //

        player.GetComponent<PlayerManager>().SetPosition(pos);
        Director.isLoading = false;

        StartCoroutine(PositionPlayer(pos));
    }

    IEnumerator PositionPlayer(Vector3 pos) {
        yield return new WaitForSeconds(0.25f);        
        //player.transform.position =  pos;
        //player.GetComponent<PlayerManager>().SetPosition(pos);
        
        uiManager.loadOverlay.SetActive(false);
    }

    //Event notifying TimeManager and CharacterStatus for curse to pause. For example when its tutorial, dialogue or trading
    public void PauseTime(bool isPaused, bool mainPause = false) {        
        if(TimePausedEvent != null) {           
            TimePausedEvent(isPaused, mainPause);
        }
    }
}
