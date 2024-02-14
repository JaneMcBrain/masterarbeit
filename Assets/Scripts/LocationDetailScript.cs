using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;

public class LocationDetailScript : MonoBehaviour
{
    public GameObject LocationPanel;
    // Start is called before the first frame update
    VisualElement rootElement;

    Location location;

    Button bookmarkBtn;
    List<string> bookmarks;

    void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;

        //Init Bookmarks
        bookmarkBtn = rootElement.Q<Button>("BookmarkButton");
        bookmarkBtn.clicked += () => toggleBookmark();
        bookmarks = SaveGameManager.CurrentActivityData.bookmarkedLocations;

        // Init BackButton
        var back_Button = rootElement.Q<Button>("BackButton");
        back_Button.clicked += () => OnBackButtonClicked();

        // Init DetailSwitchButtons
        var tour_Button = rootElement.Q<Button>("SwitchTourButton");
        var info_Button = rootElement.Q<Button>("SwitchInfoButton");
        tour_Button.clicked += () => OnSwitchButtonClicked(tour_Button, "DetailTours");
        info_Button.clicked += () => OnSwitchButtonClicked(info_Button, "DetailInfo");
    }

    public void UpdateDetailPanel(Location selectedLocation)
    {
        location = selectedLocation;

        //Overwrite the content of DetailView
        rootElement.Q<Label>("LocationNameLabel").text = selectedLocation.name;
        string imagePath = selectedLocation.image;
        if (imagePath.Length == 0)
        {
            imagePath = "Sprites/Locations/default_location";
        }
        rootElement.Q<VisualElement>("DetailHeader").style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
        rootElement.Q<Label>("InfoText").text = selectedLocation.info;
        var topicText = "Themen";
        rootElement.Q<Label>("HeadlineTour").text = topicText;
        rootElement.Q<Button>("SwitchTourButton").text = topicText;

        //Activate Bookmark
        if (bookmarks.Contains(selectedLocation.id))
        {
            bookmarkBtn.Q<VisualElement>("Icon").AddToClassList("is-active");
        }

        //check if adress is available
        if (selectedLocation.adress != null)
        {
            rootElement.Q<VisualElement>("DetailAdress").AddToClassList("show");
            rootElement.Q<Label>("StreetLabel").text = selectedLocation.adress.street;
            rootElement.Q<Label>("ZipLabel").text = selectedLocation.adress.zip + " " + selectedLocation.adress.city;
        }
    }

    void toggleBookmark(){
        if(bookmarks.Contains(location.id)){
            bookmarks.Remove(location.id);
            bookmarkBtn.Q<VisualElement>("Icon").RemoveFromClassList("is-active");
        } else {
            bookmarks.Add(location.id);
            bookmarkBtn.Q<VisualElement>("Icon").AddToClassList("is-active");
        }
        SaveGameManager.CurrentActivityData.bookmarkedLocations = bookmarks;
        SaveGameManager.SaveState();
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
