using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    //AreaManager areaManager;
    GameDirector gameDirector;
    //InteractionManager interactionManager;

    public GameObject player;
    //Each time area spawns, get all Resource objects from Resource Parent.
    //List<InteractionResource> resources = new List<InteractionResource>();
    //InteractionResource activeResource;
    //public ResourceLifeProgressBar lifeProgressBar; 

    public bool debugOn = false;



    public void Init() {
        // areaManager = GetComponent<AreaManager>();
        // GameObject managers = GameObject.FindGameObjectWithTag("Managers");
        // gameDirector = managers.GetComponent<GameDirector>();
        // interactionManager = managers.GetComponent<InteractionManager>();

        // AreaManager.AreaCreatedEvent += AreaCreated;
        // AreaManager.AreaWillGetDeleted += AreaWillGetDeleted;
        // AreaManager.ForestReshuffled += ForestReshuffled;
        // TimeManager.DayPhaseChanged += DayPhaseChanged;
    } 

    void OnDisable() {
        // AreaManager.AreaCreatedEvent -= AreaCreated;
        // AreaManager.AreaWillGetDeleted -= AreaWillGetDeleted;
        // AreaManager.ForestReshuffled += ForestReshuffled;
        // TimeManager.DayPhaseChanged -= DayPhaseChanged;
    }

    // void AreaWillGetDeleted(AreaNode areaNode) {
    //     SaveResources();
    // }

    // void AreaCreated(AreaNode areaNode) {
    //     SetupResources(GameDirector.dayPhase);
    // }      
    

    //Save States of Resources -  Each time area is left, save lifetime for resource. Have one function in SaveManager to save list ( LavenderSprout_7, 0.64, wasBlessed)
    //Add Resource object to each area

    // void Update() {

    //     //Update all Resources Life Cycle
    //     foreach(InteractionResource res in resources) {
    //         if(res.DayPhaseHidden) { continue; }
            
    //         if(res.MgOn == false && res.dailyResource == false) {
    //             res.Life += Time.deltaTime;
    //             if(res.Life > res.lifeCycle) { res.Life = res.lifeCycle; }
    //             res.SetState();
    //         }
    //     }

    //     //Find Closest Interactions to the Player 
    //     float shortestDistance = Mathf.Infinity;
    //     Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.y);
    //     int closestResourceIndex = 0;

    //     for(int i = 0; i < resources.Count; i++) {

    //         if(resources[i] == null) { continue; }
    //         if(resources[i].DayPhaseHidden) { continue; }

    //         //Vector2 resourcePos = new Vector2(resources[i].transform.position.x, resources[i].transform.position.y);
    //         Vector2 resourcePos = new Vector2(resources[i].anchor.position.x, resources[i].anchor.position.y);
    //         float distance = Vector2.Distance(playerPos, resourcePos);

    //         if(distance < shortestDistance){
    //             shortestDistance = distance;
    //             closestResourceIndex = i;
    //         }
    //     }        

    //     if(shortestDistance < Director.distanceToInteract) {    

    //         activeResource = resources[closestResourceIndex];    

    //         //Dont show life progress for daily resources
    //         if(activeResource.dailyResource == false) {
    //             lifeProgressBar.SetLifeBar(true, activeResource); 
    //         }
    //     } else {
    //         activeResource = null;
    //         lifeProgressBar.SetLifeBar(false, null);
    //     }
    // }

    // void FindResources() {
    //     GameObject temp = GameObject.FindGameObjectWithTag("Resources");
    //     resources.Clear();
    //     if(temp != null) {
    //         foreach(Transform resource in temp.transform) {
    //             if(resource.gameObject.activeSelf) {                    
    //                 InteractionResource res =  resource.GetComponent<InteractionResource>();                    
    //                 //Add resource to active resources only if its matching current day phase
    //                 //if(res.resourceDayPhase == dayPhase) {
    //                 resources.Add(res); 
    //                 //print("Found resource " + res.itemName);                   
    //                 //}
    //             }
    //         }
    //     }
    //     interactionManager.FindInteractions();
    // }

    // public void SaveResources() {     
        
    //     FindResources();
    //     if(debugOn) {
    //         Debug.Log("Saving Resources " + resources.Count);
    //     }

    //     //Save Resources into ResourceSaveObjects
    //     //Always save all resources to not lose data
    //     Dictionary<string, ResourceSaveObject> resourceSaveDict = SaveManager.LoadResources();
    //     int areaIndex = areaManager.GetCurrentArea().index;      

    //     //Go through all resources in current area, setup data and save them
    //     for(int i = 0; i < resources.Count; i++) {
    //         ResourceSaveObject saveObject = new ResourceSaveObject();
    //         string resourceName = resources[i].itemName + "_" + areaIndex + "_" + resources[i].resourceIndex;       
    //         saveObject.resourceIndex = resourceName;  //LavenderSprout_1_1
    //         saveObject.resourceLife = resources[i].Life;
    //         saveObject.dailyResource = resources[i].dailyResource;
    //         saveObject.isHidden = resources[i].IsHidden;
    //         saveObject.RolledOnHidden = resources[i].RolledOnHidden;
    //         if(resources[i].canBeCheered) {
    //             saveObject.wasCheered = resources[i].gameObject.GetComponent<InteractionCheer>().WasCheered;
    //         } else {
    //             saveObject.wasCheered = false;
    //         }
    //         saveObject.saveTime = GameDirector.GameTime;

    //         if(debugOn) { print("Saving Resource " + resourceName); }           

    //         //If this resource is already in the dict, just overwrite the data or add new resource. Save all resources for all areas
    //         if(resourceSaveDict.ContainsKey(resourceName)) {
    //             resourceSaveDict[resourceName] = saveObject;
    //         } else {
    //             resourceSaveDict.Add(resourceName, saveObject);
    //         }
    //     }

    //     SaveManager.SaveResources(resourceSaveDict);
    // }

    // //When quiting game all resources need their life and time updates based on elapsed time
    // /*
    // public void SaveAllResourcesTime() {
    //     Dictionary<string, ResourceSaveObject> resourceSaveDict = SaveManager.LoadResources();

    //     foreach(KeyValuePair<string, ResourceSaveObject> resource in resourceSaveDict) {
    //         float timeToAdd = GameDirector.GameTime - resource.Value.saveTime;
    //         resource.Value.resourceLife += timeToAdd;
    //         resource.Value.saveTime = GameDirector.GameTime;
    //     }

    //     SaveManager.SaveResources(resourceSaveDict);
    // }*/

    // void SetupResources(GameDirector.DayPhase dayPhase) {       

    //     FindResources();
    //     //Set correct resources on/off
    //     foreach(InteractionResource res in resources) {
    //         res.SetDayPhase(dayPhase);
    //     }

    //     if(debugOn) {
    //         Debug.Log("Loading Resources " + resources.Count);
    //     }

    //     //Load Resources and initialize them
    //     Dictionary<string, ResourceSaveObject> resourceSaveDict = SaveManager.LoadResources();
    //     int areaIndex = areaManager.GetCurrentArea().index;

    //     if(debugOn) {
    //         foreach(KeyValuePair<string, ResourceSaveObject> saveObj in resourceSaveDict) { print("Resource In Dict " + saveObj.Key); }
    //     }

    //     //Go through each resource in this scene, find it by its name and set its values
    //     foreach(InteractionResource resource in resources) {
    //         if(resource.resourceDayPhase != dayPhase) { continue; }

    //         string resourceName = resource.itemName + "_" + areaIndex + "_" + resource.resourceIndex;            

    //         if(resourceSaveDict.ContainsKey(resourceName)) {

    //             ResourceSaveObject saveObject = resourceSaveDict[resourceName];                           
    //             resource.SetResouceLife(saveObject.resourceLife, saveObject.saveTime);
    //             if(resource.canBeCheered) {
    //                 resource.gameObject.GetComponent<InteractionCheer>().SetCheered(saveObject.wasCheered);
    //             }
    //             if(debugOn) { Debug.Log("Resource Loaded " + resourceName + " Life " + saveObject.resourceLife + " SaveTime " + saveObject.saveTime + " Cheered " + saveObject.wasCheered.ToString()); }

    //         } else {

    //             if(debugOn) { Debug.Log("Scene resource Initialized or not Found in Saved resources " + resourceName); }
    //             float rndLifeValue = Random.Range(0f, resource.lifeCycle);
    //             resource.SetResouceLife(rndLifeValue, 0f); //First time encountering the resource, set life to rnd value
    //         }

    //         //Value is hidden, actualy hide it
    //         if(resource.IsHidden) { resource.SetHidden(); }

    //         //If its daily resource, have only some chance to actualy have the resource show up
    //         //Roll if its visible or not. Roll only once per day. Gets reseted each day
    //         if(resource.dailyResource && resource.RolledOnHidden == false) {
    //             float roll = Random.Range(0f, 1f);
    //             if(roll > 0.4f) {
    //                 resource.SetHidden();
    //             }
    //             resource.RolledOnHidden = true;
    //         }           
    //     }        
    // }
  

    // //When new day starts, reset daily resource to life == 1
    // void DayPhaseChanged(GameDirector.DayPhase dayPhase) {       

    //     //If its day, set daily resources
    //     if(dayPhase == GameDirector.DayPhase.Day) {
    //         Dictionary<string, ResourceSaveObject> resourceSaveDict = SaveManager.LoadResources();
    //         foreach(var saveObj in resourceSaveDict) {
                
    //                 if(saveObj.Value.dailyResource) {                    
    //                     saveObj.Value.resourceLife = 1f;
    //                     saveObj.Value.isHidden = false;
    //                     saveObj.Value.RolledOnHidden = false;
    //                 }
                
    //         }
    //         SaveManager.SaveResources(resourceSaveDict);   
    //     } 

    //     //Save all resources at the end of the dayphase, so that their states get stored at the end of the day        
    //     SetupResources(dayPhase);  
    //     SaveResources();         
    // }

    // void ForestReshuffled() {
    //     SaveResources();
    // }
}

[System.Serializable]
public class ResourceSaveObject {
    public string resourceIndex;
    public float resourceLife;
    public bool wasCheered;
    public bool dailyResource;  //Resource that can be collected each day, resets life with new day
    public bool isHidden;
    public bool RolledOnHidden;
    public float saveTime;      //GameTime when the data were last saved. Use this to update life value to current time, if player was in other areas
}
