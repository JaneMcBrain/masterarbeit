using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;

public class TopicDetailScript : MonoBehaviour
{
    public GameObject LocationDetailPanel;

    VisualElement rootElement;
    Topic topic;
    Button bookmarkBtn;
    List<string> bookmarks;
    void OnEnable()
    {
        rootElement = GetComponent<UIDocument>().rootVisualElement;

        //Init Bookmarks
        bookmarkBtn = rootElement.Q<Button>("BookmarkButton");
        bookmarkBtn.clicked += () => toggleBookmark();
        bookmarks = SaveGameManager.CurrentActivityData.bookmarkedTopics;

        // Init BackButton
        var back_Button = rootElement.Q<Button>("BackButton");
        back_Button.clicked += () => gameObject.SetActive(false);

        // Init DetailSwitchButtons
        var tour_Button = rootElement.Q<Button>("SwitchTourButton");
        var info_Button = rootElement.Q<Button>("SwitchInfoButton");
        tour_Button.clicked += () => OnSwitchButtonClicked(tour_Button, "DetailTours");
        info_Button.clicked += () => OnSwitchButtonClicked(info_Button, "DetailInfo");
    }

    public void UpdateDetailPanel(Topic selectedTopic)
    {
        topic = selectedTopic;
        //Overwrite the content of DetailView
        rootElement.Q<Label>("TopicNameLabel").text = selectedTopic.name;
        string imagePath = selectedTopic.image;
        if (imagePath.Length == 0)
        {
            imagePath = "Sprites/Topics/default_tour";
        }
        Debug.Log(imagePath);
        rootElement.Q<VisualElement>("DetailHeader").style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
        rootElement.Q<Label>("InfoText").text = selectedTopic.info;

        //Activate Bookmark
        if (bookmarks.Contains(selectedTopic.id))
        {
            bookmarkBtn.Q<VisualElement>("Icon").AddToClassList("is-active");
        }
    }

    void toggleBookmark()
    {
        if (bookmarks.Contains(topic.id))
        {
            bookmarks.Remove(topic.id);
            bookmarkBtn.Q<VisualElement>("Icon").RemoveFromClassList("is-active");
        }
        else
        {
            bookmarks.Add(topic.id);
            bookmarkBtn.Q<VisualElement>("Icon").AddToClassList("is-active");
        }
        SaveGameManager.CurrentActivityData.bookmarkedTopics = bookmarks;
        SaveGameManager.SaveState();
    }

    // Update is called once per frame
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
}
