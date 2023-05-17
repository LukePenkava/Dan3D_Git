using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaManager : MonoBehaviour
{
    Areas currentArea = Areas.none;
    Areas prevArea = Areas.none; 
    public Areas PrevArea { get { return prevArea; }}  

    public void LoadArea(Areas areaToLoad) {

        if(currentArea != Areas.none) {
            SceneManager.UnloadSceneAsync(currentArea.ToString());
        }        

        prevArea = currentArea;
        currentArea = areaToLoad;
        print("currentArea " + currentArea.ToString());
        SceneManager.LoadScene(currentArea.ToString(), LoadSceneMode.Additive);
    }
}
