using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocationDetailScript : MonoBehaviour
{
    public GameObject LocationPanel;
    // Start is called before the first frame update
    void OnEnable()
    {
        var rootElement = GetComponent<UIDocument>().rootVisualElement;
        var back_Button = rootElement.Q<Button>("BackButton");
        back_Button.clicked += () => OnBackButtonClicked();
    }

    void OnBackButtonClicked(){
        gameObject.SetActive(false);
        LocationPanel.SetActive(true);
    }
}
