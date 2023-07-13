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

    // Start is called before the first frame update
    void Start()
    {
        var rootElement = footer_uiDocument.rootVisualElement;
        var home_Button = rootElement.Q<Button>("HomeButton");
        var location_Button = rootElement.Q<Button>("LocationsButton");
        var tour_Button = rootElement.Q<Button>("ToursButton");
        var account_Button = rootElement.Q<Button>("AccountButton");
        var info_Button = rootElement.Q<Button>("InfoButton");
        home_Button.clicked += () => OnFooterButtonClicked("start", home_Button);
        home_Button.AddToClassList("is-active");
        location_Button.clicked += () => OnFooterButtonClicked("location", location_Button);
        tour_Button.clicked += () => OnFooterButtonClicked("tour", tour_Button);
        account_Button.clicked += () => OnFooterButtonClicked("account", account_Button);
        info_Button.clicked += () => OnFooterButtonClicked("info", info_Button);
    }



    private void OnFooterButtonClicked(string type, Button btn)
    {
        setPanelActive(type);
        btn.AddToClassList("is-active");
    }

    private void setPanelActive(string panel){
        VisualElement result = footer_uiDocument.rootVisualElement.Q(className: "is-active");
        if(result != null){
            result.RemoveFromClassList("is-active");
        }
        StartPanel.SetActive(panel == "start");
        LocationPanel.SetActive(panel == "location");
        TourPanel.SetActive(panel == "tour");
        AccountPanel.SetActive(panel == "account");
        InfoPanel.SetActive(panel == "info");
    }
}
