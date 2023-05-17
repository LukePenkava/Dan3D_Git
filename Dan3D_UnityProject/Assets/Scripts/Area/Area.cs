using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public delegate void AreaDelegate(Area areaScript);
    public static event AreaDelegate AreaLoaded;

    AreaManager areaScript;    
    public List<AreaSpawnPos> spawnPositins = new List<AreaSpawnPos>();

    public List<Digsite> digsitesList = new List<Digsite>();
    public int digsitesAmount = 2;

    //Create timer for digsites and save their state, like 24 hours ingame time to refresh availability

    void Start() {

        if(AreaLoaded != null) {           
            AreaLoaded(this);
        }

        for(int i = 0; i < digsitesAmount; i++) {
            Digsite newDigsite = new Digsite();
            digsitesList.Add(newDigsite);
        }
    }

    public int AvailableDigSites() {
        int amount = 0;

        for(int i = 0; i < digsitesList.Count; i++) {
            if(digsitesList[i].isAvailable) {
                amount++;
            }
        }

        return amount;
    }

    public void DigsiteCompleted() {
        for(int i = 0; i < digsitesList.Count; i++) {
            if(digsitesList[i].isAvailable) {
                digsitesList[i].isAvailable = false;
                return;
            }
        }
    }
}

public class Digsite {
    public bool isAvailable = true;
}
