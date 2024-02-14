using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class LocationListController
{
    // UXML template for list entries
    VisualTreeAsset LocationEntryTemplate;
    VisualTreeAsset TopicEntryTemplate;
    VisualTreeAsset TourEntryTemplate;

    // UI element references
    ListView LocationList;
    Label LocationNameLabel;
    VisualElement LocationImage;
    List<Location> AllLocations;
    List<Topic> AllTopics;
    List<Tour> AllTours;
    GameObject LocationListPanel;
    GameObject LocationDetailPanel;
    GameObject TopicDetailPanel;

    public void InitializeLocationList(UIDocument uiDocument, VisualTreeAsset listElementTemplate, VisualTreeAsset topicElementTemplate, VisualTreeAsset tourElementTemplate, List<Location> locations, GameObject detailPage, List<Topic> topics, GameObject topicDetailPage, List<Tour> tours)
    {
        AllLocations =  locations;
        AllTopics = topics;
        AllTours = tours;
        var root = uiDocument.rootVisualElement;
        // Save the elements inside the class
        LocationEntryTemplate = listElementTemplate;
        TopicEntryTemplate = topicElementTemplate;
        TourEntryTemplate = tourElementTemplate;
        LocationDetailPanel = detailPage;
        TopicDetailPanel = topicDetailPage;
        LocationListPanel = uiDocument.gameObject;

        // Get the UXML Elements
        LocationList = root.Q<ListView>("LocationList");

        FillLocationList();

        // Register to get a callback when an item is selected
        LocationList.onSelectionChange += OnLocationSelected;
    }

    void FillLocationList()
    {
        // Set up a make item function for a list entry
        LocationList.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = LocationEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new LocationListEntryController();

            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;

            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };

        // Set up bind function for a specific list entry
        LocationList.bindItem = (item, index) =>
        {
            (item.userData as LocationListEntryController).SetLocationData(AllLocations[index]);
        };

        // Set a fixed item height
        LocationList.fixedItemHeight = 150;

        // Set the actual item's source list/array
        LocationList.itemsSource = AllLocations;
    }

    void OnLocationSelected(IEnumerable<object> selectedItems)
    {
        // Get the currently selected item directly from the ListView
        var selectedLocation = LocationList.selectedItem as Location;
        
        // Hide List and show detail panel
        LocationListPanel.SetActive(false);
        LocationDetailPanel.SetActive(true);

        // Find the LocationDetailScript component attached to the detail panel's UIDocument
        LocationDetailScript locationDetailScript = LocationDetailPanel.GetComponent<LocationDetailScript>();

        if (locationDetailScript != null)
        {
            // Call the UpdateDetailPanel method to update the detail panel
            locationDetailScript.UpdateDetailPanel(selectedLocation);
        }

        //Overwrite the content of DetailView
        var detailUi = LocationDetailPanel.GetComponent<UIDocument>().rootVisualElement;

        //Filter Topics via Location ID
        List<Topic> locationTopics = new List<Topic>();
        foreach (var topic in AllTopics)
        {
            if (topic.location == selectedLocation.id)
            {
                locationTopics.Add(topic);
            }
        }

        var topicListController = new TopicListController();
        topicListController.InitializeTopicList(
            detailUi,
            TopicEntryTemplate,
            locationTopics,
            LocationDetailPanel,
            TopicDetailPanel,
            AllTours,
            TourEntryTemplate
        );

    }
}
