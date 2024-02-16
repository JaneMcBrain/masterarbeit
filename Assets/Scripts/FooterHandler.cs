using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class FooterHandler : MonoBehaviour
{
   public UIDocument footer_uiDocument;
   public GameObject StartPanel;
   public GameObject LocationPanel;
   public GameObject TourPanel;
   public GameObject AccountPanel;
   public GameObject InfoPanel;
   public GameObject LocationDetailPanel;
   public GameObject TopicDetailPanel;
    private VisualElement rootElement;

    // Start is called before the first frame update
    void Start()
    {
        rootElement = footer_uiDocument.rootVisualElement;
        var home_Button = rootElement.Q<Button>("HomeButton");
        var location_Button = rootElement.Q<Button>("LocationsButton");
        var tour_Button = rootElement.Q<Button>("ToursButton");
        var account_Button = rootElement.Q<Button>("AccountButton");
        var info_Button = rootElement.Q<Button>("InfoButton");
        home_Button.clicked += () => OnFooterButtonClicked("Home", home_Button);
        location_Button.clicked += () => OnFooterButtonClicked("Locations", location_Button);
        tour_Button.clicked += () => OnFooterButtonClicked("Tours", tour_Button);
        account_Button.clicked += () => OnFooterButtonClicked("Account", account_Button);
        info_Button.clicked += () => OnFooterButtonClicked("Info", info_Button);
        setBtnActive("Home");
    }



    private void OnFooterButtonClicked(string type, Button btn)
    {
        StartPanel.SetActive(type == "Home");
        LocationPanel.SetActive(type == "Locations");
        TourPanel.SetActive(type == "Tours");
        AccountPanel.SetActive(type == "Account");
        InfoPanel.SetActive(type == "Info");
        LocationDetailPanel.SetActive(false);
        TopicDetailPanel.SetActive(false);
        setBtnActive(type);
    }

    public void setBtnActive(string type){
        VisualElement result = footer_uiDocument.rootVisualElement.Q<VisualElement>(className: "is-active");
        if (result != null)
        {
            result.RemoveFromClassList("is-active");
        }
        rootElement.Q<Button>(type + "Button").Q<VisualElement>("Icon").AddToClassList("is-active");
    }
}
