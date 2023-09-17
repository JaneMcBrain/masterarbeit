using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LoadExercises : MonoBehaviour
{
    public TextAsset jsonFile;
    public GameObject ExerciseTextView;

    [SerializeField]
    VisualTreeAsset ListEntryTemplate;

    void OnEnable()
    {

        Tours toursInJson = JsonUtility.FromJson<Tours>(jsonFile.text);
        //get selected Tour
        var tourId = "tour1";
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
