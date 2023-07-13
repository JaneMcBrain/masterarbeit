using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TourListController
{
    // UXML template for list entries
    VisualTreeAsset ListEntryTemplate;

    // UI element references
    ListView TourList;
    Label TourNameLabel;
    Label TourProgressLabel;
    VisualElement CharPortrait;
    List<Tour> AllTours;

    public void InitializeTourList(VisualElement root, VisualTreeAsset listElementTemplate, List<Tour> tours)
    {
        AllTours =  tours;

        // Store a reference to the template for the list entries
        ListEntryTemplate = listElementTemplate;

        // Store a reference to the Tour list element
        TourList = root.Q<ListView>("TourList");

        // Store references to the selected character info elements
        TourNameLabel = root.Q<Label>("TourName");
        TourProgressLabel = root.Q<Label>("TourTours");

        FillTourList();

        // Register to get a callback when an item is selected
        //TourList.onSelectionChange += OnTourSelected;
    }

    void FillTourList()
    {
        // Set up a make item function for a list entry
        TourList.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = ListEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new TourListEntryController();

            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;

            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };

        // Set up bind function for a specific list entry
        TourList.bindItem = (item, index) =>
        {
            (item.userData as TourListEntryController).SetTourData(AllTours[index]);
        };

        // Set a fixed item height
        TourList.fixedItemHeight = 110;

        // Set the actual item's source list/array
        TourList.itemsSource = AllTours;
    }
}
