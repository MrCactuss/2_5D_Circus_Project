using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using static UnityEditor.PlayerSettings;
using UnityEngine.TextCore.Text;

public class SaveLoadScript : MonoBehaviour
{
    public string saveFileName = "saveFile.json";

    [Serializable]
    public class GameData
    {
        public int character;
        public string characterName;
        //.....
    }

    private GameData gameData = new GameData();

    public void SaveGame(int character, string name)
    {
        gameData.character = character;
        gameData.characterName = name;

        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(Application.persistentDataPath + "/" + saveFileName, json);
        Debug.Log("Game saved to: " + Application.persistentDataPath + "/" + saveFileName);
    }

    public void LoadGame()
    {
        string filePath = Application.persistentDataPath + "/" + saveFileName;

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Game loaded from: " + Application.persistentDataPath + "/" + saveFileName);


        }
        else
            Debug.LogWarning("Save file not foaund: " + filePath);
    }
}