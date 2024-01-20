using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Area : MonoBehaviour
{
    AreaManager areaManager;  

    public Areas area;
    public Locations location;
    //public List<AreaSpawnPos> spawnPositins = new List<AreaSpawnPos>();
    public Transform spawnPositions;

    //public List<Digsite> digsitesList = new List<Digsite>();
    //public int digsitesAmount = 2;

    //Create timer for digsites and save their state, like 24 hours ingame time to refresh availability

    //Camera Settings
    //public float camera_AngleX = 2f;
    //public float camera_DistanceZ = 6.2f;
    //public float camera_VerticalOffset = 0.6f;

    //public Volume volume;
   

    void Start() {

        this.transform.position = Vector3.zero;        

        areaManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<AreaManager>();
        areaManager.NewAreaLoadedEvent(this);

        // for(int i = 0; i < digsitesAmount; i++) {
        //     Digsite newDigsite = new Digsite();
        //     digsitesList.Add(newDigsite);
        // }

        //SetQuality();
    }

    // public int AvailableDigSites() {
    //     int amount = 0;

    //     for(int i = 0; i < digsitesList.Count; i++) {
    //         if(digsitesList[i].isAvailable) {
    //             amount++;
    //         }
    //     }

    //     return amount;
    // }

    // public void DigsiteCompleted() {
    //     for(int i = 0; i < digsitesList.Count; i++) {
    //         if(digsitesList[i].isAvailable) {
    //             digsitesList[i].isAvailable = false;
    //             return;
    //         }
    //     }
    // }

    // public void SetQuality() {
    //     if(Director.quality == "medium") {          
    //         GlobalIllumination gi;
    //         volume.profile.TryGet<GlobalIllumination>(out gi);
    //         gi.active = false;
    //     }

    //     if(Director.quality == "high") {          
    //         GlobalIllumination gi;
    //         volume.profile.TryGet<GlobalIllumination>(out gi);
    //         gi.active = true;
    //     }
        
    // }
}

// public class Digsite {
//     public bool isAvailable = true;
// }
