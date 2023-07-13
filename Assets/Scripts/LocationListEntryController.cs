using UnityEngine.UIElements;

public class LocationListEntryController
{
    Label NameLabel;
    Label TourLabel;

    //This function retrieves a reference to the 
    //character name label inside the UI element.

    public void SetVisualElement(VisualElement visualElement)
    {
        NameLabel = visualElement.Q<Label>("LocationName");
        TourLabel = visualElement.Q<Label>("LocationTours");
    }

    //This function receives the character whose name this list 
    //element displays. Since the elements listed 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.

    public void SetLocationData(Location location)
    {
        NameLabel.text = location.name;
        TourLabel.text = location.tours.Length.ToString();
    }
}
