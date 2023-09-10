using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class LocationListController
{
    // UXML template for list entries
    VisualTreeAsset ListEntryTemplate;

    // UI element references
    ListView LocationList;
    Label LocationNameLabel;
    Label LocationTourLabel;
    VisualElement LocationImage;
    List<Location> AllLocations;
    GameObject LocationListPanel;
    GameObject LocationDetailPanel;


    public void InitializeLocationList(UIDocument uiDocument, VisualTreeAsset listElementTemplate, List<Location> locations, GameObject detailPage)
    {
        AllLocations =  locations;
        var root = uiDocument.rootVisualElement;
        // Save the elements inside the class
        ListEntryTemplate = listElementTemplate;
        LocationDetailPanel = detailPage;
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
            var newListEntry = ListEntryTemplate.Instantiate();

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

        //Overwrite the content of DetailView
        var detailUi = LocationDetailPanel.GetComponent<UIDocument>().rootVisualElement;
        detailUi.Q<Label>("LocationNameLabel").text = selectedLocation.name;
        string imagePath = selectedLocation.image;
        if (imagePath.Length == 0)
        {
            imagePath = "Sprites/Locations/default_location";
        }
        detailUi.Q<VisualElement>("DetailHeader").style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
    }
}
