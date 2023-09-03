using UnityEngine.UIElements;
using UnityEngine;
using System;

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

    public void SetLocationData(Location location)
    {
        LocationName.text = location.name;
        LocationTour.text = "Touren: " + location.tours.Length.ToString();
        string imagePath = location.image;
        if(imagePath.Length > 0) {
            LocationImage.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
        }
    }
}
