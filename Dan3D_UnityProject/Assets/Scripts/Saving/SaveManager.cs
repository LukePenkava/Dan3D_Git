using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveManager 
{
    public static void ResetAll() {
        ResetItems();
        ResetCharacterData();
        ResetResources();
        SaveGameTime(0);
        SaveGameData(new GameData());
        SaveQuestData(new QuestData());
    }

    public static void SaveItems(Characters.Names character, List<InventoryItem> itemsList) {    
        string path = GetItemsPath(character);   
              
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        bf.Serialize(file, itemsList);
        file.Close();
    }

    public static List<InventoryItem> LoadItems(Characters.Names character) {
        string path = GetItemsPath(character);

        if(File.Exists(path)) {     
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            List<InventoryItem> itemsList = (List<InventoryItem>)bf.Deserialize(file);
            file.Close();

            return itemsList;
        } else {
            List<InventoryItem> emptyList = new List<InventoryItem>();
            return emptyList;
        }
    }

    public static void ResetItems() {
        var characterNames = Characters.Names.GetValues(typeof(Characters.Names));
        foreach(Characters.Names charName in characterNames) {            
            List<InventoryItem> emptyList = new List<InventoryItem>();
            SaveItems(charName, emptyList);
        }
    }

    static string GetItemsPath(Characters.Names character) {
        return Application.persistentDataPath + "/" + character.ToString() + "_Items.dan";
    }

    static string GetCharacterDataPath(Characters.Names character) {
        return Application.persistentDataPath + "/" + character.ToString() + "_CharacterData.dan";
    }

    //Save character data like dialogue progression, last state/position/area etc
    public static void SaveCharacterData(Characters.Names character, CharacterData data) {
        string path = GetCharacterDataPath(character);   

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        bf.Serialize(file, data);
        file.Close();
    }

     public static CharacterData LoadCharacterData(Characters.Names character) {
        string path = GetCharacterDataPath(character);

        if(File.Exists(path)) {     
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            CharacterData data = (CharacterData)bf.Deserialize(file);
            file.Close();

            return data;
        } else {
            return new CharacterData();
        }
    }

    public static void ResetCharacterData() {
        var characterNames = Characters.Names.GetValues(typeof(Characters.Names));
        foreach(Characters.Names charName in characterNames) {
             SaveCharacterData(charName, new CharacterData());
        }
    }

    public static void SaveGameData(GameData data) {
        string path = Application.persistentDataPath + "/GameData.dan";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        bf.Serialize(file, data);
        file.Close();
    }

     public static GameData LoadGameData() {
        string path = Application.persistentDataPath + "/GameData.dan";

        if(File.Exists(path)) {     
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            GameData data = (GameData)bf.Deserialize(file);
            file.Close();

            return data;
        } else {
            return new GameData();
        }
    }

    public static void SaveQuestData(QuestData data) {
        string path = Application.persistentDataPath + "/QuestData.dan";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(path);
        bf.Serialize(file, data);
        file.Close();
    }

    public static QuestData LoadQuestData() {
        string path = Application.persistentDataPath + "/QuestData.dan";

        if(File.Exists(path)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            QuestData data = (QuestData)bf.Deserialize(file);
            file.Close();

            return data;
        } else {
            return new QuestData();
        }
    }

    public static void SaveResources(Dictionary<string, ResourceSaveObject> resourceSaveObjects) {

        string savePath = Application.persistentDataPath + "/Res.dan";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, resourceSaveObjects);
        file.Close();
    }

    public static Dictionary<string, ResourceSaveObject> LoadResources() {

        string savePath = Application.persistentDataPath + "/Res.dan";

        if(File.Exists(savePath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            Dictionary<string, ResourceSaveObject> resources = (Dictionary<string, ResourceSaveObject>)bf.Deserialize(file);
            file.Close();

            return resources;

        } else {
            Dictionary<string, ResourceSaveObject> resources = new Dictionary<string, ResourceSaveObject>();
            return resources;
        }
    }

    public static void ResetResources() {

        Dictionary<string, ResourceSaveObject> res = new Dictionary<string, ResourceSaveObject>();
        res.Add("null", new ResourceSaveObject());        
        SaveResources(res);
    }

    public static void SaveGameTime(float gameTime) {
        string savePath = Application.persistentDataPath + "/GameTime.dan";

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(savePath);
        bf.Serialize(file, gameTime);
        file.Close();
    }

    public static float LoadGameTime() {
        string savePath = Application.persistentDataPath + "/GameTime.dan";

        if(File.Exists(savePath)) {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(savePath, FileMode.Open);
            float gameTime = (float)bf.Deserialize(file);
            file.Close();

            return gameTime;

        } else {
            Debug.Log("Failed to load GameTime");
            return 0f;
        }
    }
}
