using UnityEngine.UIElements;
using UnityEditor;
using UnityEngine;

public class TourListEntryController
{
    Label NameLabel;
    Label ProgressLabel;
    VisualElement TourImage;

    //This function retrieves a reference to the 
    //character name label inside the UI element.

    public void SetVisualElement(VisualElement visualElement)
    {
        NameLabel = visualElement.Q<Label>("TourName");
        ProgressLabel = visualElement.Q<Label>("TourProgress");
        TourImage = visualElement.Q<VisualElement>("TourImage");
    }

    //This function receives the character whose name this list 
    //element displays. Since the elements listed 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.

    public void SetTourData(Tour tour)
    {
        NameLabel.text = tour.name;
        ProgressLabel.text = tour.progress + "%";
        TourImage.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>("Sprites/Test"));
    }
}
