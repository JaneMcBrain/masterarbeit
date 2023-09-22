using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class TourListEntryController
{
    Label NameLabel;
    Label ProgressLabel;
    VisualElement TourImage;

    public void SetVisualElement(VisualElement visualElement)
    {
        NameLabel = visualElement.Q<Label>("TourName");
        ProgressLabel = visualElement.Q<Label>("TourProgress");
        TourImage = visualElement.Q<VisualElement>("TourImage");
    }
    public void SetTourData(Tour tour)
    {
        NameLabel.text = tour.name;
        ProgressLabel.text = tour.progress + "%";
        string imagePath = tour.image;
        if (imagePath.Length > 0)
        {
            TourImage.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
        }
    }
}
