using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

[System.Serializable]
public class Tours
{
    //employees is case sensitive and must match the string "employees" in the JSON.
    public List<Tour> tours;
}

public class TourReader : MonoBehaviour
{
    public TextAsset jsonFile;

    [SerializeField]
    VisualTreeAsset ListEntryTemplate;
 
    void Start()
    {
        Tours toursInJson = JsonUtility.FromJson<Tours>(jsonFile.text);

        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        // Initialize the character list controller
        var tourListController = new TourListController();
        tourListController.InitializeTourList(uiDocument.rootVisualElement, ListEntryTemplate, toursInJson.tours);
    }
}
