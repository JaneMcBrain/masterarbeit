using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class TourReader : MonoBehaviour
{
    public TextAsset jsonFile;

    [SerializeField]
    VisualTreeAsset ListEntryTemplate;
 
    void OnEnable()
    {
        Tours toursInJson = JsonUtility.FromJson<Tours>(jsonFile.text);
        var uiDocument = GetComponent<UIDocument>();
        var tourListController = new TourListController();
        tourListController.InitializeTourList(uiDocument.rootVisualElement, ListEntryTemplate, toursInJson.tours, "TourList");
    }
}
