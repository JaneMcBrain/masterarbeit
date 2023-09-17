using System.IO;
using UnityEngine;
namespace SaveLoadSystem
{
  public static class SaveGameManager{
    public static SaveData CurrentSaveData = new SaveData();
    public const string directoryPath = "/Data/";
    public const string FileName = "user.json";
    public static bool SaveState(){
      var dir = Application.persistentDataPath + directoryPath;
      if (!Directory.Exists(dir)){
        Directory.CreateDirectory(dir);
      }
      string json = JsonUtility.ToJson(CurrentSaveData, true);
      File.WriteAllText(dir + FileName, json);
      GUIUtility.systemCopyBuffer = dir;
      return true;
    }

    public static bool LoadState(){
      string fullPath = Application.persistentDataPath + directoryPath + FileName;
      SaveData tempData = new SaveData();
      if (File.Exists(fullPath))
      {
        string json = File.ReadAllText(fullPath);
        tempData = JsonUtility.FromJson<SaveData>(json);
      } else {
        Debug.Log("File does not exists");
      }
      CurrentSaveData = tempData;
      return true;
    }
  }
}
