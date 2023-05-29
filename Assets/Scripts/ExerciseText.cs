using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExerciseText : MonoBehaviour
{
    public TMP_Text content;
    // Start is called before the first frame update
    void Start()
    {
        int currentLevel = PlayerPrefs.GetInt("level");
        if(currentLevel == 1){
            content.text = "Das erste Gemälde diente einst als Zielscheibe in einem Schützenverein.\nFinde ein Gemälde mit zwei Vögeln.";
        }
        if(currentLevel == 2){
            content.text = "Finde den Sohn von Johann Sebastian Back, welcher an einem Cembalo sitzend nicht so viel von den musikalischen Künsten seines Arbeitgebers hält.";
        }
        if(currentLevel == 3){
            content.text = "Finde die Parade auf dem Opernplatz in Berlin.";
        }
    }
}
