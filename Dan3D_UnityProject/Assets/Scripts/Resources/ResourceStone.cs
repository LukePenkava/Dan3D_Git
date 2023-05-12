using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceStone : Resource
{
    MinigameManager mgManager;

    public GameObject bigRock;
    public GameObject smallRocks;
    public GameObject _collider;

    void Start() 
    {
        mgManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<MinigameManager>();

        bigRock.SetActive(true);
        smallRocks.SetActive(false);

        _collider.gameObject.SetActive(true);
    }

    public override void Collect()
    {        
        GameDirector.gameState = GameDirector.GameState.MG;
        mgManager.mgDelegate = MinigameResult;
        mgManager.Init();        
    }

    //Minigame, he should put his arm up with animation, stop, minigame starts, so that player can tell based on animation when it will happen, then if success, on hit sparkles and better sound. If faile normal sound, no sparkles
    //If this happens on having arm up, there is no collider to check hit, so there has to be precheck with other collider where the hit will land on press attack to see where frying pan will land.

    public void MinigameResult(bool result) {

        print("result " + result);

        List<Items.ItemName> itemList = new List<Items.ItemName>();
        itemList.Add(Items.ItemName.Stone); 
        //Give second stone, if success
        if(result) {     
            itemList.Add(Items.ItemName.Stone);    
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");              
        player.GetComponent<Character_BaseData>().AddItems(itemList, true);
        
        bigRock.SetActive(false);
        smallRocks.SetActive(true);
        _collider.SetActive(false);

        GameDirector.gameState = GameDirector.GameState.World;
    }


}
