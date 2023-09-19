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
        //Save Exercise via GameManager
        SaveGameManager.CurrentSaveData.currentExercise = selectedExercise;
        SaveGameManager.SaveState();
        ExerciseTextView.SetActive(true);
    }
}
