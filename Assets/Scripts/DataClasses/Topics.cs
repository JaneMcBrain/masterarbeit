using System.Collections.Generic;

[System.Serializable]
public class Topics
{
  public List<Topic> topics;
}
[System.Serializable]
public class Topic
{
  public string id;
  public string location;
  public string name;
  public string image;
  public string info;
  public string progress;
}
