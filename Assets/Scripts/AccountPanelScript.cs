using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;

public class AccountPanelScript : MonoBehaviour
{
    private UIDocument uiDocument;
    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        setActivityText();
        Button resetBtn = uiDocument.rootVisualElement.Q<Button>("ResetActivity");
        resetBtn.clicked += () => resetActivity();
    }

    void setActivityText(){
        uiDocument.rootVisualElement.Q<Label>("JsonText").text = JsonUtility.ToJson(SaveGameManager.CurrentActivityData, true);
    }

    void resetActivity(){
        SaveGameManager.DeleteSavedData();
        setActivityText();
    }
}
