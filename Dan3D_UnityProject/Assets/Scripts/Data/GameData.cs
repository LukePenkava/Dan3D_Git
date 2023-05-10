using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData {

    public bool firstLaunch = true;

    //Tutorial
    public int tutorialIndex = 1;
    public bool tutorial_1_active = true;  
    

    //GameData saves also all player's progress
    public Dictionary<string, bool> unlockedRecipes = new Dictionary<string, bool>();

}