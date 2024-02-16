using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using SaveLoadSystem;

public class ExerciseTextStart : MonoBehaviour
{
    public TextAsset jsonFile;
    public GameObject TourEndView;
    private string tourId;
    // Start is called before the first frame update
    void OnEnable()
    {
        SaveGameManager.LoadState();
        //get current exercise
        var currentEx = SaveGameManager.CurrentActivityData.currentExercise;
        //get selected Tour
        tourId = SaveGameManager.CurrentActivityData.currentTour;

        Tours toursInJson = JsonUtility.FromJson<Tours>(jsonFile.text);
        Tour selectedTour = toursInJson.tours.Find(t => t.id.Contains(tourId));
        SaveGameManager.CurrentActivityData.StartExercise(selectedTour);
        SaveGameManager.SaveState();
        if(currentEx.tourId == ""){
            TourEndView.SetActive(true);
        } else {
            setExerciseText();
        }
    }

    void setExerciseText(){
        var exerciseTextUi = gameObject.GetComponent<UIDocument>().rootVisualElement;
        exerciseTextUi.Q<Label>("ExerciseText").text = SaveGameManager.CurrentActivityData.currentExercise.exercise.text;
        exerciseTextUi.Q<Button>("ExerciseStartButton").clicked += () => startExercise();
        exerciseTextUi.Q<Button>("BackButton").clicked += () => onBackClick();
    }

    void startExercise(){
        SceneManager.LoadScene("ScanScene");
    }

    void onBackClick(){
        string exerciseId = SaveGameManager.CurrentActivityData.currentExercise.exercise.id;
        SaveGameManager.CurrentActivityData.openExercises.Find(e => e.tourId == tourId).exerciseIds.Insert(0, exerciseId);
        //Remove finished here or in Interaction-Back Btn
        SaveGameManager.CurrentActivityData.currentExercise = null;
        SaveGameManager.SaveState();
        SceneManager.LoadScene("NavigationScene");
    }
}
