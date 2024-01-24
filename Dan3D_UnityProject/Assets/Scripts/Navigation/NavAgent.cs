using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavAgent : MonoBehaviour
{
    NavManager navManager;
    public List<NavVoxel> CurrentPath { get; private set; } = null;

    public bool Inited { get; private set; } = false;
    public bool debugPath = false;


    void OnEnable()
    {
        AreaManager.AreaLoaded += AreaLoadedInit;
    }

    void OnDisable()
    {
        AreaManager.AreaLoaded -= AreaLoadedInit;
    }


    void AreaLoadedInit(Area areaScript)
    {
        navManager = GameObject.FindGameObjectWithTag("NavManager").GetComponent<NavManager>();
        Inited = true;
    }

    public void GetPath(Vector3 pathStart, Vector3 pathTarget)
    {
        CurrentPath = navManager.GetPath(pathStart, pathTarget);        
    }

    void OnDrawGizmos()
    {
        if (debugPath == false) { return; }
        if (CurrentPath == null) { return; }

        for (int i = 0; i < CurrentPath.Count; i++)
        {
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            Gizmos.DrawSphere(CurrentPath[i].WorldPosition, 0.5f);

            if(i > 0) {
                Gizmos.DrawLine(CurrentPath[i-1].WorldPosition, CurrentPath[i].WorldPosition);
            }
        }
    }
}
