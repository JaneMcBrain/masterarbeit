using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class LocationReader : MonoBehaviour
{
    public TextAsset jsonLocationFile;
    public TextAsset jsonTopicFile;
    public TextAsset jsonTourFile;
    public GameObject LocationDetailPanel;
    public GameObject TopicDetailPanel;

    [SerializeField]
    VisualTreeAsset ListEntryTemplate;
    [SerializeField]
    VisualTreeAsset TopicEntryTemplate;
    [SerializeField]
    VisualTreeAsset TourEntryTemplate;

    void OnEnable()
    {
        Locations locationsInJson = JsonUtility.FromJson<Locations>(jsonLocationFile.text);
        Topics topicsInJson = JsonUtility.FromJson<Topics>(jsonTopicFile.text);
        Tours toursInJson = JsonUtility.FromJson<Tours>(jsonTourFile.text);

        // The UXML is already instantiated by the UIDocument component
        var uiDocument = GetComponent<UIDocument>();

        // Initialize the character list controller
        var locationListController = new LocationListController();
        locationListController.InitializeLocationList(
            uiDocument,
            ListEntryTemplate,
            TopicEntryTemplate,
            TourEntryTemplate,
            locationsInJson.locations,
            LocationDetailPanel,
            topicsInJson.topics,
            TopicDetailPanel,
            toursInJson.tours
        );
    }
}
