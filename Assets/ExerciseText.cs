using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExerciseText : MonoBehaviour
{
    public TMP_Text exerciseText;
    // Start is called before the first frame update
    void Awake()
    {
        int currentLevel = PlayerPrefs.GetInt("level");
        exerciseText = GetComponent<TextMeshProUGUI>();
        if(currentLevel == 0){
            exerciseText.text = "Das erste Gemälde diente einst als Zielscheibe in einem Schützenverein. Finde ein Gemälde mit zwei Vögeln.";
        }
        if(currentLevel == 1){
            exerciseText.text = "Flötenkonzert Aufgabe";
        }
        if(currentLevel == 2){
            exerciseText.text = "Objektaustausch Aufgabe";
        }
    }
}
