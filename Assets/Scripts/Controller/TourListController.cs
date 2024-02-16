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

    public void InitializeTourList(
        VisualElement root,
        VisualTreeAsset listElementTemplate,
        List<Tour> tours,
        string listName
    )
    {
        AllTours =  tours;
        ListEntryTemplate = listElementTemplate;
        TourList = root.Q<ListView>(listName);
        FillTourList();
    }

    void FillTourList()
    {
        TourList.makeItem = () =>
        {
            var newListEntry = ListEntryTemplate.Instantiate();
            var newListEntryLogic = new TourListEntryController();
            newListEntry.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElement(newListEntry);
            return newListEntry;
        };
        TourList.bindItem = (item, index) =>
        {
            (item.userData as TourListEntryController).SetTourData(AllTours[index]);
        };
        TourList.fixedItemHeight = 150;
        TourList.onSelectionChange += OnTourSelected;
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
        SaveGameManager.CurrentActivityData.StartTour(selectedTour.id);
        SaveGameManager.SaveState();
        SceneManager.LoadScene("InteractionNavi");
    }
}
