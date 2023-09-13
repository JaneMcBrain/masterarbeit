using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class TopicListEntryController
{
    Label NameLabel;
    Label ProgressLabel;
    VisualElement TopicImage;

    //This function retrieves a reference to the 
    //character name label inside the UI element.

    public void SetVisualElement(VisualElement visualElement)
    {
        NameLabel = visualElement.Q<Label>("TopicName");
        ProgressLabel = visualElement.Q<Label>("TopicProgress");
        TopicImage = visualElement.Q<VisualElement>("TopicImage");
    }

    //This function receives the character whose name this list 
    //element displays. Since the elements listed 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.

    public void SetTopicData(Topic topic)
    {
        NameLabel.text = topic.name;
        ProgressLabel.text = topic.progress + "%";
        string imagePath = topic.image;
        if (imagePath.Length > 0)
        {
            TopicImage.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
        }
    }
}
