using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartPanelScript : MonoBehaviour
{
    public UIDocument start_uiDocument;
    public GameObject LocationPanel;
    public GameObject TourPanel;

    void Start()
    {
        var rootElement = start_uiDocument.rootVisualElement;
        var location_Button = rootElement.Q<Button>("LocationButton");
        var tour_Button = rootElement.Q<Button>("TourButton");
        location_Button.clicked += () => LocationPanel.SetActive(true);
        tour_Button.clicked += () => TourPanel.SetActive(true);
    }
}
