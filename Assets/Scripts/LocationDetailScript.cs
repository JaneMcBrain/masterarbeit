using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class LocationDetailScript : MonoBehaviour
{
    public GameObject LocationPanel;
    // Start is called before the first frame update
    VisualElement rootElement;

    void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;
        
        // Init BackButton
        var back_Button = rootElement.Q<Button>("BackButton");
        back_Button.clicked += () => OnBackButtonClicked();

        // Init DetailSwitchButtons
        var tour_Button = rootElement.Q<Button>("SwitchTourButton");
        var info_Button = rootElement.Q<Button>("SwitchInfoButton");
        tour_Button.clicked += () => OnSwitchButtonClicked(tour_Button, "DetailTours");
        info_Button.clicked += () => OnSwitchButtonClicked(info_Button, "DetailInfo");
    }

    void OnSwitchButtonClicked(Button btn, string name)
    {
        var activeButton = rootElement.Q<Button>(className: "is-active");
        if (activeButton != null)
        {
            activeButton.RemoveFromClassList("is-active");
        }
        var activeDetail = rootElement.Q<VisualElement>(className: "is-active");
        if (activeDetail != null)
        {
            activeDetail.RemoveFromClassList("is-active");
        }
        rootElement.Q<VisualElement>(name).AddToClassList("is-active");
        btn.AddToClassList("is-active");
    }

    void OnBackButtonClicked(){
        gameObject.SetActive(false);
        LocationPanel.SetActive(true);
    }
}
