using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;

public class LoadExercises : MonoBehaviour
{
    public TextAsset jsonFile;
    public GameObject ExerciseTextView;

    [SerializeField]
    VisualTreeAsset ListEntryTemplate;

    void OnEnable()
    {
        //get current exercise
        var currentEx = SaveGameManager.CurrentActivityData.currentExercise;
        //get selected Tour
        var tourId = SaveGameManager.CurrentActivityData.currentTour;
        //avoid jumping into ExerciseText Panel, when we have a different tour id
        if (currentEx != null && currentEx.tourId == tourId)
        {
            ExerciseTextView.SetActive(true);
        }
        Tours toursInJson = JsonUtility.FromJson<Tours>(jsonFile.text);
        
        //Filter Tours via Topic ID
        List<Exercise> tourExercises = new List<Exercise>();
        Tour selectedTour = toursInJson.tours.Find(t => t.id.Contains(tourId) );
        if (selectedTour != null)
        {
            tourExercises = selectedTour.exercises;
        }
        var uiDocument = GetComponent<UIDocument>();
        var exerciseListController = new ExerciseListController();
        exerciseListController.InitializeExerciseList(uiDocument.rootVisualElement, ListEntryTemplate, tourExercises, ExerciseTextView);
    }
}
