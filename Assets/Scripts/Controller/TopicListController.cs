using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System;

public class TopicListController
{
    // UXML template for list entries
    VisualTreeAsset ListEntryTemplate;
    VisualTreeAsset TourEntryTemplate;

    // UI element references
    ListView TopicList;
    Label TopicNameLabel;
    Label TopicProgressLabel;
    VisualElement CharPortrait;
    List<Topic> AllTopics;
    List<Tour> AllTours;
    GameObject LocationDetailPanel;
    GameObject TopicDetailPanel;

    public void InitializeTopicList(VisualElement root, VisualTreeAsset listElementTemplate, List<Topic> topics, GameObject locationDetailPanel, GameObject topicDetailPanel, List<Tour> tours, VisualTreeAsset tourElementTemplate)
    {
        AllTopics =  topics;
        AllTours = tours;

        // Store a reference to the template for the list entries
        ListEntryTemplate = listElementTemplate;
        TourEntryTemplate = tourElementTemplate;
        TopicDetailPanel = topicDetailPanel;
        LocationDetailPanel = locationDetailPanel;

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

        // Hide List and show detail panel
        TopicDetailPanel.SetActive(true);

        //Overwrite the content of DetailView
        var detailUi = TopicDetailPanel.GetComponent<UIDocument>().rootVisualElement;
        detailUi.Q<Label>("TopicNameLabel").text = selectedTopic.name;
        string imagePath = selectedTopic.image;
        if (imagePath.Length == 0)
        {
            imagePath = "Sprites/Topics/default_tour";
        }
        Debug.Log(imagePath);
        detailUi.Q<VisualElement>("DetailHeader").style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
        detailUi.Q<Label>("InfoText").text = selectedTopic.info;

        //Filter Tours via Topic ID
        List<Tour> topicTours = new List<Tour>();
        foreach (var tour in AllTours)
        {
            string availableTopic = Array.Find(tour.topics, t => t == selectedTopic.id);
            if (availableTopic != null)
            {
                topicTours.Add(tour);
            }
        }

        var tourListController = new TourListController();
        tourListController.InitializeTourList(
            detailUi,
            TourEntryTemplate,
            topicTours,
            "DetailList"
        );
    }
}
