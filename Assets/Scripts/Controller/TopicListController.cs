using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class TopicListController
{
    // UXML template for list entries
    VisualTreeAsset ListEntryTemplate;

    // UI element references
    ListView TopicList;
    Label TopicNameLabel;
    Label TopicProgressLabel;
    VisualElement CharPortrait;
    List<Topic> AllTopics;

    public void InitializeTopicList(VisualElement root, VisualTreeAsset listElementTemplate, List<Topic> topics)
    {
        AllTopics =  topics;

        // Store a reference to the template for the list entries
        ListEntryTemplate = listElementTemplate;

        // Store a reference to the Topic list element
        TopicList = root.Q<ListView>("DetailList");

        // Store references to the selected character info elements
        TopicNameLabel = root.Q<Label>("TopicName");
        TopicProgressLabel = root.Q<Label>("TopicTopics");

        FillTopicList();

        // Register to get a callback when an item is selected
        //TopicList.onSelectionChange += OnTopicSelected;
    }

    void FillTopicList()
    {
        // Set up a make item function for a list entry
        TopicList.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = ListEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new TopicListEntryController();

            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;

            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };

        // Set up bind function for a specific list entry
        TopicList.bindItem = (item, index) =>
        {
            (item.userData as TopicListEntryController).SetTopicData(AllTopics[index]);
        };

        // Set a fixed item height
        TopicList.fixedItemHeight = 150;

        // Register to get a callback when an item is selected
        TopicList.onSelectionChange += OnTopicSelected;

        // Set the actual item's source list/array
        TopicList.itemsSource = AllTopics;
    }

     void OnTopicSelected(IEnumerable<object> selectedItems)
    {
        // Get the currently selected item directly from the ListView
        var selectedTopic = TopicList.selectedItem as Topic;

        // Handle none-selection (Escape to deselect everything)
        if (selectedTopic == null)
        {
            return;
        }

        // Fill in character details
        SceneManager.LoadScene("InteractionNavi");
    }
}
