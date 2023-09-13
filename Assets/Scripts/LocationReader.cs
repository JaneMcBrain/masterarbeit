using UnityEngine;
using UnityEngine.UIElements;
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

public class LocationReader : MonoBehaviour
{
    public TextAsset jsonLocationFile;
    public TextAsset jsonTopicFile;
    public GameObject LocationDetailPanel;

    [SerializeField]
    VisualTreeAsset ListEntryTemplate;
    [SerializeField]
    VisualTreeAsset TopicEntryTemplate;

    void OnEnable()
    {
        Locations locationsInJson = JsonUtility.FromJson<Locations>(jsonLocationFile.text);
        Topics topicsInJson = JsonUtility.FromJson<Topics>(jsonTopicFile.text);
        Debug.Log(topicsInJson.topics);

        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        // Initialize the character list controller
        var locationListController = new LocationListController();
        locationListController.InitializeLocationList(
            uiDocument,
            ListEntryTemplate,
            TopicEntryTemplate,
            locationsInJson.locations,
            LocationDetailPanel,
            topicsInJson.topics
        );
    }
}
