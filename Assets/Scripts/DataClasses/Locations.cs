using System.Collections.Generic;

[System.Serializable]
public class Locations
{
  public List<Location> locations;
}

[System.Serializable]
public class Location
{
  public string id;
  public string name;
  public string image;
  public string[] tours;
  public Adress adress;
  public string info;
}

[System.Serializable]
public class Adress
{
  public string street;
  public string zip;
  public string city;
  public string country;
}
