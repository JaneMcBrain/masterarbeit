using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;
public class BookmarkScript : MonoBehaviour
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
    Tours toursInJson = JsonUtility.FromJson<Tours>(jsonTourFile.text);
    Locations locationsInJson = JsonUtility.FromJson<Locations>(jsonLocationFile.text);
    Topics topicsInJson = JsonUtility.FromJson<Topics>(jsonTopicFile.text);
    //get the started exercises
    SaveGameManager.LoadState();
    var bookmarkedTopicIds = SaveGameManager.CurrentActivityData.bookmarkedTopics;
    var bookmarkedLocationIds = SaveGameManager.CurrentActivityData.bookmarkedLocations;
    List<Topic> bookmarkedTopics = new List<Topic>();
    foreach (var topic in topicsInJson.topics)
    {
      if (bookmarkedTopicIds.Contains(topic.id))
      {
        bookmarkedTopics.Add(topic);
      }
    }
    List<Location> bookmarkedLocations = new List<Location>();
    foreach (var location in locationsInJson.locations)
    {
      if (bookmarkedLocationIds.Contains(location.id))
      {
        bookmarkedLocations.Add(location);
      }
    }
    var uiDocument = GetComponent<UIDocument>();
    var topicListController = new TopicListController();
    topicListController.InitializeTopicList(
        uiDocument.rootVisualElement,
        TopicEntryTemplate,
        bookmarkedTopics,
        LocationDetailPanel,
        TopicDetailPanel,
        toursInJson.tours,
        TourEntryTemplate
    );
    var locationListController = new LocationListController();
    locationListController.InitializeLocationList(
        uiDocument,
        ListEntryTemplate,
        TopicEntryTemplate,
        TourEntryTemplate,
        bookmarkedLocations,
        LocationDetailPanel,
        topicsInJson.topics,
        TopicDetailPanel,
        toursInJson.tours
    );
  }
}
