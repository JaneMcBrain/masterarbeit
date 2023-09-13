using UnityEngine;
using UnityEngine.UIElements;
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
}

public class TourReader : MonoBehaviour
{
    public TextAsset jsonFile;

    [SerializeField]
    VisualTreeAsset ListEntryTemplate;
 
    void OnEnable()
    {
        Tours toursInJson = JsonUtility.FromJson<Tours>(jsonFile.text);

        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        // Initialize the character list controller
        var tourListController = new TourListController();
        tourListController.InitializeTourList(uiDocument.rootVisualElement, ListEntryTemplate, toursInJson.tours, "TourList");
    }
}
