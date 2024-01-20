using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

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

    //Temp Quest
    bool questCompleted = false;
    int wood = 0;
    int woodAmount = 4;
    int stone = 0;
    int stoneAmount = 3;
    int crunchyCrystal = 0;
    int crunchyCrystalAmount = 2;
    int everflour = 0;
    int everflourAmount = 1;
    public Text questText1;
    public Text questText2;
    public Text resourcesText;    


    void OnEnable() {
        //AreaManager.AreaLoaded += AreaLoaded;
    }

    void OnDisable() {
        //AreaManager.AreaLoaded -= AreaLoaded;
    }

    // Start is called before the first frame update
    public void Init()
    {
        GameDirector.gameState = GameDirector.GameState.World;

        uiManager = GetComponent<UIManager>();
        areaManager = GetComponent<AreaManager>();

        // uiManager.loadOverlay.SetActive(true);
        // areaManager.LoadArea(firstAreaToLoad);

        //PerformDynamicRes scaler = PerformDynamicRes.
        //DynamicResolutionHandler.SetDynamicResScaler()
        //print(ItemsDirectory.ItemsData[Items.ItemName.Wood].description);

        //Setup Dans starting items
        // List<Items.ItemName> itemList = new List<Items.ItemName>();
        // itemList.Add(Items.ItemName.Wood);               
        //itemList.Add(Items.ItemName.Stardew);               
        //player.GetComponent<Character_BaseData>().AddItems(itemList);    

        //Temp
        // questText2.gameObject.SetActive(false);    
        // questText1.gameObject.SetActive(true); 
        // resourcesText.gameObject.SetActive(true);    
        // resourcesText.text = $"Stone {stone}/{stoneAmount}, Wood {wood}/{woodAmount}, Crunchy Crystal {crunchyCrystal}/{crunchyCrystalAmount}, Everflour {everflour}/{everflourAmount}";
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
                zima.GetComponent<CharacterController>().enabled = false;
                zima.transform.position = new Vector3(-0.45f, 0.21f, -2.67f);
                zima.transform.localEulerAngles = new Vector3(0,0,0);
                zima.GetComponent<CharacterController>().enabled = true;
                zima.GetComponent<Zima>().SetHome(true);
            }
        }

        
        //player.transform.position =  new Vector3(pos.x, pos.y, pos.z);     
        //

        player.GetComponent<PlayerManager>().SetPosition(pos);
        Director.isLoading = false;

        StartCoroutine(PositionPlayer(pos));

        //Temp Quest
        if(areaScript.area == Areas.Home_3) {
            GameObject[] interactions = GameObject.FindGameObjectsWithTag("Interaction");
            foreach(GameObject temp in interactions) {
                if(temp.name == "InteractionCook") {
                    temp.gameObject.SetActive(questCompleted);
                }
            }
        }
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

   

    //Temp Quest
    public void PlayerReceivedItem(Items.ItemName itemName) {
        switch(itemName) {
            case Items.ItemName.Stone:
                stone++;    
                if(stone > stoneAmount) { stone = stoneAmount; }            
                break;
            case Items.ItemName.Wood:
                wood++;
                if(wood > woodAmount) { wood = woodAmount; }  
                break;
            case Items.ItemName.Everflour:
                everflour++;
                if(everflour > everflourAmount) { everflour = everflourAmount; }  
                break;
            case Items.ItemName.CrunchyCrystal:
                crunchyCrystal++;
                if(crunchyCrystal > crunchyCrystalAmount) { crunchyCrystal = crunchyCrystalAmount; }  
                break;
            default:
                break;

        }

        resourcesText.text = $"Stone {stone}/{stoneAmount}, Wood {wood}/{woodAmount}, Crunchy Crystal {crunchyCrystal}/{crunchyCrystalAmount}, Everflour {everflour}/{everflourAmount}";

        if(stone >= stoneAmount && wood >= woodAmount && crunchyCrystal >= crunchyCrystalAmount && everflour >= everflourAmount) {
            resourcesText.gameObject.SetActive(false);
            questText1.gameObject.SetActive(false);
            questText2.gameObject.SetActive(true);
            questCompleted = true;
        }
    }

    public void Cook() {
        Director.isLoading = true;
        uiManager.SetEndGameScreen();
    }
}
