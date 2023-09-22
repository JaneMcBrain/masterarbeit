using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using SaveLoadSystem;

public class TourListController
{
    // UXML template for list entries
    VisualTreeAsset ListEntryTemplate;

    // UI element references
    ListView TourList;
    VisualElement CharPortrait;
    List<Tour> AllTours;

    public void InitializeTourList(VisualElement root, VisualTreeAsset listElementTemplate, List<Tour> tours, string listName)
    {
        AllTours =  tours;

        // Store a reference to the template for the list entries
        ListEntryTemplate = listElementTemplate;

        // Store a reference to the Tour list element
        TourList = root.Q<ListView>(listName);

        FillTourList();
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
        TourList.fixedItemHeight = 150;

        // Register to get a callback when an item is selected
        TourList.onSelectionChange += OnTourSelected;

        // Set the actual item's source list/array
        TourList.itemsSource = AllTours;
    }

     void OnTourSelected(IEnumerable<object> selectedItems)
    {
        // Get the currently selected item directly from the ListView
        var selectedTour = TourList.selectedItem as Tour;

        // Handle none-selection (Escape to deselect everything)
        if (selectedTour == null)
        {
            return;
        }

        SaveGameManager.LoadState();
        SaveGameManager.CurrentActivityData.currentTour = selectedTour.id;
        SaveGameManager.SaveState();
        SceneManager.LoadScene("InteractionNavi");
    }
}
