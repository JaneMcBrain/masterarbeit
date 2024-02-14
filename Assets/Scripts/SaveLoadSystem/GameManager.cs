using System.IO;
using UnityEngine;
namespace SaveLoadSystem
{
  public static class SaveGameManager{
    public static ActivityData CurrentActivityData = new ActivityData();
    public const string directoryPath = "/Data/";
    public const string FileName = "user.json";
    public static bool SaveState(){
      var dir = Application.persistentDataPath + directoryPath;
      if (!Directory.Exists(dir)){
        Directory.CreateDirectory(dir);
      }
      string json = JsonUtility.ToJson(CurrentActivityData, true);
      File.WriteAllText(dir + FileName, json);
      GUIUtility.systemCopyBuffer = dir;
      return true;
    }

    public static bool LoadState(){
      string fullPath = Application.persistentDataPath + directoryPath + FileName;
      ActivityData tempData = new ActivityData();
      if (File.Exists(fullPath))
      {
        string json = File.ReadAllText(fullPath);
        tempData = JsonUtility.FromJson<ActivityData>(json);
      } else {
        Debug.Log("File does not exists");
      }
      CurrentActivityData = tempData;
      return true;
    }

    // Methode zum Löschen der gespeicherten Daten
    public static bool DeleteSavedData()
    {
      string fullPath = Application.persistentDataPath + directoryPath + FileName;
      if (File.Exists(fullPath))
      {
        File.Delete(fullPath);
        // Optional: Zurücksetzen der CurrentActivityData auf den Standardzustand
        CurrentActivityData = new ActivityData();
        return true;
      }
      Debug.Log("No saved data to delete.");
      return false;
    }
  }
}
