using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;

public class ActivityScript : MonoBehaviour
{
  public TextAsset jsonFile;

  [SerializeField]
  VisualTreeAsset ListEntryTemplate;

  void OnEnable()
  {
    Tours toursInJson = JsonUtility.FromJson<Tours>(jsonFile.text);
    //get the started exercises
    SaveGameManager.LoadState();
    var startedTourIds = SaveGameManager.CurrentActivityData.activeTours;
    List<Tour> startedTours = new List<Tour>();
    foreach (var tour in toursInJson.tours)
    {
      if (startedTourIds.Contains(tour.id))
      {
        startedTours.Add(tour);
      }
    }
    var uiDocument = GetComponent<UIDocument>();
    var tourListController = new TourListController();
    tourListController.InitializeTourList(uiDocument.rootVisualElement, ListEntryTemplate, startedTours, "ActivityList");
  }
}
