using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;


public class ExerciseListController
{
    // UXML template for list entries
    VisualTreeAsset ListEntryTemplate;

    // UI element references
    ListView ExerciseList;
    Label ExerciseButton;
    List<Exercise> AllExercises;

    GameObject ExerciseTextView;

    public void InitializeExerciseList(VisualElement root, VisualTreeAsset listElementTemplate, List<Exercise> exercises, GameObject exerciseTextView)
    {
        AllExercises =  exercises;
        ListEntryTemplate = listElementTemplate;
        ExerciseList = root.Q<ListView>("ExerciseList");
        ExerciseTextView = exerciseTextView;
        FillExerciseList();
        ExerciseList.onSelectionChange += OnExerciseSelected;
    }

    void FillExerciseList()
    {
        // Set up a make item function for a list entry
        ExerciseList.makeItem = () =>
        {
            var newListEntry = ListEntryTemplate.Instantiate();
            ExerciseButton = newListEntry.Q<Label>("ExerciseName");
            return ExerciseButton;
        };


        ExerciseList.bindItem = (item, index) =>
        {
            (item as Label).text = AllExercises[index].name;
        };

        ExerciseList.itemsSource = AllExercises;
    }

     void OnExerciseSelected(IEnumerable<object> selectedItems)
    {
        var selectedExercise = ExerciseList.selectedItem as Exercise;
        if (selectedExercise == null)
        {
            return;
        }
        //Save current Exercise via GameManager
        var currentTour = SaveGameManager.CurrentActivityData.currentTour;
        var currentEx = new ActiveExercise() { tourId = currentTour, exercise = selectedExercise };
        SaveGameManager.CurrentActivityData.currentExercise = currentEx;
        var startedExercises = SaveGameManager.CurrentActivityData.activeExercises;
        //Add current Exercise to activeExercises, if not yet saved
        //find activeExercise
        var activeExercise = startedExercises.Find(x => x.tourId == SaveGameManager.CurrentActivityData.currentTour);
        if (activeExercise == null){
            SaveGameManager.CurrentActivityData.activeExercises.Add(currentEx);
        } else if(activeExercise.exercise != selectedExercise)
        {
            //Overwrite existing activeExercise with certain tourId
            var index = SaveGameManager.CurrentActivityData.activeExercises.IndexOf(activeExercise);
            SaveGameManager.CurrentActivityData.activeExercises[index].exercise = selectedExercise;
        }
        SaveGameManager.SaveState();
        ExerciseTextView.SetActive(true);
    }
}
