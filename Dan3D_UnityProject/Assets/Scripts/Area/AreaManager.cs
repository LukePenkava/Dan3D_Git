using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaManager : MonoBehaviour
{
    public delegate void AreaDelegate(Area areaScript);
    public static event AreaDelegate AreaLoaded;


    UIManager uiManager;

    Areas currentArea = Areas.none;
    public Areas CurrentArea { get { return currentArea; }}  
    Areas prevArea = Areas.none; 
    public Areas PrevArea { get { return prevArea; }}  

    

    public void LoadArea(Areas areaToLoad) {

        Director.isLoading = true;   
        
        if(uiManager == null) {
            uiManager = GetComponent<UIManager>();
        }
        uiManager.loadOverlay.SetActive(true);

        if(currentArea != Areas.none) {
            SceneManager.UnloadSceneAsync(currentArea.ToString());
        }           

        prevArea = currentArea;
        currentArea = areaToLoad;
        SceneManager.LoadScene(currentArea.ToString(), LoadSceneMode.Additive);
        

       //StartCoroutine(LoadYourAsyncScene(currentArea.ToString()));
    }    

    // IEnumerator LoadYourAsyncScene(string sceneToLoad)
    // {
    //     // The Application loads the Scene in the background as the current Scene runs.
    //     // This is particularly good for creating loading screens.
    //     // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
    //     // a sceneBuildIndex of 1 as shown in Build Settings.

    //     AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

    //     // Wait until the asynchronous scene fully loads
    //     while (!asyncLoad.isDone)
    //     {
    //         yield return null;
    //     }

    //     print("Async Finished");
    //     Director.isLoading = false;  

    //     Area areaScript = GameObject.FindGameObjectWithTag("Area").GetComponent<Area>(); 

    //     if(AreaLoaded != null) {           
    //         AreaLoaded(areaScript);
    //     }

    // }

    public void NewAreaLoadedEvent(Area areaScript) {
        if(AreaLoaded != null) {           
            AreaLoaded(areaScript);
        }        
    }
}
