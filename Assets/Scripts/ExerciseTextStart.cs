using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using SaveLoadSystem;

public class ExerciseTextStart : MonoBehaviour
{
    // Start is called before the first frame update
    void OnEnable()
    {
        var exerciseTextUi = gameObject.GetComponent<UIDocument>().rootVisualElement;
        //get the active Exercise from GameManager
        exerciseTextUi.Q<Label>("ExerciseText").text = SaveGameManager.CurrentActivityData.currentExercise.exercise.text;
        exerciseTextUi.Q<Button>("ExerciseStartButton").clicked += () => SceneManager.LoadScene("ScanScene");
        exerciseTextUi.Q<Button>("BackButton").clicked += () => onBackClick();
    }

    void onBackClick(){
        SaveGameManager.CurrentActivityData.currentExercise = null;
        SaveGameManager.SaveState();
        gameObject.SetActive(false);
    }
}
