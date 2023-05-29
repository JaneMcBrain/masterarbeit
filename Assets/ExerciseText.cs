using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExerciseContent : MonoBehaviour
{
    public TMP_Text content;
    // Start is called before the first frame update

    void Start()
    {
        int currentLevel = PlayerPrefs.GetInt("level");
        content = GetComponent<TextMeshProUGUI>();
        if(currentLevel == 0){
            content.text = "Das erste Gemälde diente einst als Zielscheibe in einem Schützenverein. Finde ein Gemälde mit zwei Vögeln.";
        }
        if(currentLevel == 1){
            content.text = "Flötenkonzert Aufgabe";
        }
        if(currentLevel == 2){
            content.text = "Objektaustausch Aufgabe";
        }
        GameObject myGO;
        GameObject myText;
        Canvas myCanvas;
        Text text;
        RectTransform rectTransform;

        // Canvas
        myGO = new GameObject();
        myGO.name = "TestCanvas";
        myGO.AddComponent<Canvas>();

        myCanvas = myGO.GetComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        myGO.AddComponent<CanvasScaler>();
        myGO.AddComponent<GraphicRaycaster>();

        // Text
        myText = new GameObject();
        myText.transform.parent = myGO.transform;
        myText.name = "wibble";

        text = myText.AddComponent<Text>();
        text.font = (Font)Resources.Load("MyFont");
        text.text = "wobble";
        text.fontSize = 100;

        // Text position
        rectTransform = text.GetComponent<RectTransform>();
        rectTransform.localPosition = new Vector3(0, 0, 0);
        rectTransform.sizeDelta = new Vector2(400, 200);
    }
}
