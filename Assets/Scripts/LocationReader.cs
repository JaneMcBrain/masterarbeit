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
    public Tour[] tours;
}

[System.Serializable]
public class Tour
{
    public string id;
    public string[] topcis;
    public string name;
    public string image;
    public string info;
    public string progress;
}

public class LocationReader : MonoBehaviour
{
    public TextAsset jsonFile;

    [SerializeField]
    VisualTreeAsset ListEntryTemplate;
 
    void OnEnable()
    {
        Locations locationsInJson = JsonUtility.FromJson<Locations>(jsonFile.text);
        print("Start Locations");
        print(locationsInJson);
        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        // Initialize the character list controller
        var locationListController = new LocationListController();
        locationListController.InitializeLocationList(uiDocument.rootVisualElement, ListEntryTemplate, locationsInJson.locations);
    }
}
