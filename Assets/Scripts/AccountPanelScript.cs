using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;

public class AccountPanelScript : MonoBehaviour
{
    public TextAsset jsonFile;

    [SerializeField]
    VisualTreeAsset ListEntryTemplate;

    void OnEnable()
    {
        Tours toursInJson = JsonUtility.FromJson<Tours>(jsonFile.text);
        //get the started exercises
        SaveGameManager.LoadState();
        var startedExercises = SaveGameManager.CurrentActivityData.activeExercises;
        List<Tour> startedTours = new List<Tour>();
        foreach (var tour in toursInJson.tours)
        {
            if (startedExercises.Find(x => x.tourId == tour.id) != null)
            {
                startedTours.Add(tour);
            }
        }
        var uiDocument = GetComponent<UIDocument>();
        var tourListController = new TourListController();
        uiDocument.rootVisualElement.Q<Label>("JsonText").text = JsonUtility.ToJson(SaveGameManager.CurrentActivityData.currentExercise, true);
        tourListController.InitializeTourList(uiDocument.rootVisualElement, ListEntryTemplate, startedTours, "ActivityList");
    }
}
