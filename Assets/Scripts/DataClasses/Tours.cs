using System.Collections.Generic;

[System.Serializable]
public class Tours
{
  //employees is case sensitive and must match the string "employees" in the JSON.
  public List<Tour> tours;
}

[System.Serializable]
public class Tour
{
  public string id;
  public string location;
  public string[] topics;
  public string name;
  public string image;
  public string info;
  public string progress;
  public List<Exercise> exercises;
}

