using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitializeManager : MonoBehaviour
{
    GameDirector gameDirector;
    CameraManager cameraManager;
    UIManager uiManager;

    //InteractionManager interactionManager;    
    //UIManager uiManager;
    //DialogueManager dialogueManager;
    //TradeManager tradeManager; 
    //AreaManager areaManager;
    //ResourceManager resourceManager;
    //HomeManager homeManager;
    //CharactersManager charactersManager;
    //TimeManager timeManager;   
    //QuestManager questManager;

    void Awake()
    {
        //Get References
        gameDirector = GetComponent<GameDirector>();
        cameraManager = GetComponent<CameraManager>();
        uiManager = GetComponent<UIManager>();

        // interactionManager = GetComponent<InteractionManager>();
        // tradeManager = GetComponent<TradeManager>();
        // uiManager = GetComponent<UIManager>();
        // dialogueManager = GetComponent<DialogueManager>();
        // areaManager = GetComponent<AreaManager>();
        // resourceManager = GetComponent<ResourceManager>();
        // homeManager = GetComponent<HomeManager>();
        // charactersManager = GetComponent<CharactersManager>();
        // timeManager = GetComponent<TimeManager>();
        // questManager = GetComponent<QuestManager>();
        // camFollow = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraFollow>();

        //Initialize Scripts
        ItemsDirectory.Init();
        
        gameDirector.Init();        
        cameraManager.Init();
        uiManager.Init();

        // resourceManager.Init(); 
        // questManager.Init();       
        // tradeManager.Init();
        // uiManager.Init();
        // dialogueManager.Init();
        // areaManager.Init();
        // interactionManager.Init(); 
        // homeManager.Init();  
        // charactersManager.Init();        

        //Characters
        //GameObject.FindGameObjectWithTag("Player").GetComponent<BaseData>().Init();
        //Rest of characters are initialized in GameDirector in Setup after Interactions are found ( characters are interactions )       

        //Area gets created here, any scripts requiring area to be spawned, need to come after this
        //gameDirector.Setup();

        //TimeManager gets inited in GameDirector SetGameStart
        cameraManager.Init();
          
    }
}