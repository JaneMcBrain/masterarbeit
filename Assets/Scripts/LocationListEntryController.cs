using UnityEngine.UIElements;
using UnityEngine;

public class LocationListEntryController
{
    Label LocationName;
    Label LocationTour;
    VisualElement LocationImage;

    //This function retrieves a reference to the 
    //character name label inside the UI element.

    public void SetVisualElement(VisualElement visualElement)
    {
        LocationName = visualElement.Q<Label>("LocationName");
        LocationTour = visualElement.Q<Label>("LocationTours");
        LocationImage = visualElement.Q<VisualElement>("LocationImage");
    }

    //This function receives the character whose name this list 
    //element displays. Since the elements listed 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.

    public void SetLocationData(Location location)
    {
        LocationName.text = location.name;
        LocationTour.text = "Touren: " + location.tours.Length.ToString();
        LocationImage.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>("Sprites/Test"));
    }
}
