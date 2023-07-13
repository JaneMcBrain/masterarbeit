using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocationListController
{
    // UXML template for list entries
    VisualTreeAsset ListEntryTemplate;

    // UI element references
    ListView LocationList;
    Label LocationNameLabel;
    Label LocationTourLabel;
    VisualElement CharPortrait;
    List<Location> AllLocations;

    public void InitializeLocationList(VisualElement root, VisualTreeAsset listElementTemplate, List<Location> locations)
    {
        AllLocations =  locations;

        // Store a reference to the template for the list entries
        ListEntryTemplate = listElementTemplate;

        // Store a reference to the Location list element
        LocationList = root.Q<ListView>("LocationList");

        // Store references to the selected character info elements
        LocationNameLabel = root.Q<Label>("LocationName");
        LocationTourLabel = root.Q<Label>("LocationTours");

        FillLocationList();

        // Register to get a callback when an item is selected
        //LocationList.onSelectionChange += OnLocationSelected;
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
        LocationList.fixedItemHeight = 45;

        // Set the actual item's source list/array
        LocationList.itemsSource = AllLocations;
    }
}
