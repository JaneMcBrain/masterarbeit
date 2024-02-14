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
        //get current exercise
        var currentEx = SaveGameManager.CurrentActivityData.currentExercise;
        //get selected Tour
        tourId = SaveGameManager.CurrentActivityData.currentTour;
        //avoid jumping into ExerciseText Panel, when we have a different tour id
        if (currentEx != null && currentEx.tourId == tourId)
        {
            setExerciseText();
        } else {
            Tours toursInJson = JsonUtility.FromJson<Tours>(jsonFile.text);

            //Filter Tours via Topic ID
            List<Exercise> tourExercises = new List<Exercise>();
            Tour selectedTour = toursInJson.tours.Find(t => t.id.Contains(tourId));
            if (selectedTour != null)
            {
                tourExercises = selectedTour.exercises;
            }
            string nextExerciseId = SaveGameManager.CurrentActivityData.GetRandomExerciseByTourIdAndRemove(tourId);
            //currentEx setzen
            Exercise nextExercise = tourExercises.Find(e => e.id.Contains(nextExerciseId));
            if(nextExercise != null){
                currentEx = new ActiveExercise() { tourId = tourId, exercise = nextExercise };
                SaveGameManager.CurrentActivityData.currentExercise = currentEx;

                var startedExercises = SaveGameManager.CurrentActivityData.activeExercises;
                var activeExercise = startedExercises.Find(x => x.tourId == SaveGameManager.CurrentActivityData.currentTour);
                if (activeExercise == null)
                {
                    SaveGameManager.CurrentActivityData.activeExercises.Add(currentEx);
                }
                else if (activeExercise.exercise != nextExercise)
                {
                    //Overwrite existing activeExercise with certain tourId
                    var index = SaveGameManager.CurrentActivityData.activeExercises.IndexOf(activeExercise);
                    SaveGameManager.CurrentActivityData.activeExercises[index].exercise = nextExercise;
                }
                SaveGameManager.SaveState();
                setExerciseText();
            } else {
                TourEndView.SetActive(true);
            }
        }
    }

    void setExerciseText(){
        //get the active Exercise from GameManager
        SaveGameManager.LoadState();
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
        SaveGameManager.CurrentActivityData.currentExercise = null;
        SaveGameManager.SaveState();
        SceneManager.LoadScene("NavigationScene");
    }
}
