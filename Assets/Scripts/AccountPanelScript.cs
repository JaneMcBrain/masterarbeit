using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;

public class AccountPanelScript : MonoBehaviour
{
    void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.Q<Label>("JsonText").text = JsonUtility.ToJson(SaveGameManager.CurrentActivityData.currentExercise, true);
    }
}
