using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class StartPanelScript : MonoBehaviour
{
    public UIDocument start_uiDocument;
    public GameObject LocationPanel;
    public GameObject TourPanel;
    public GameObject Footer;
    private FooterHandler footerScript;

    void OnEnable()
    {
        footerScript = Footer.GetComponent<FooterHandler>();
        var rootElement = start_uiDocument.rootVisualElement;
        var location_Button = rootElement.Q<Button>("LocationButton");
        var tour_Button = rootElement.Q<Button>("TourButton");
        location_Button.clicked += () => onLocationClick();
        tour_Button.clicked += () => onTourClick();
    }

    private void onLocationClick(){
        gameObject.SetActive(false);
        LocationPanel.SetActive(true);
        footerScript.setBtnActive("Locations");
    }

    private void onTourClick(){
        gameObject.SetActive(false);
        TourPanel.SetActive(true);
        footerScript.setBtnActive("Tours");
    }
}
