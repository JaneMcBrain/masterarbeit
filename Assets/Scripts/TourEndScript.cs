using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;
using UnityEngine.SceneManagement;

public class TourEndScript : MonoBehaviour
{
    public TextAsset jsonFile;
    void OnEnable()
    {
        Tours toursInJson = JsonUtility.FromJson<Tours>(jsonFile.text);
        string tourId = SaveGameManager.CurrentActivityData.currentTour;
        Tour currentTour = new Tour();
        foreach (var tour in toursInJson.tours)
        {
            if (tourId.Contains(tour.id))
            {
                currentTour = tour;
            }
        }
        string imagePath = currentTour.image;
        if (imagePath.Length == 0)
        {
            imagePath = "Sprites/Tour/default_tour";
        }
        var root = gameObject.GetComponent<UIDocument>().rootVisualElement;
        root.Q<VisualElement>("TourEndImage").style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
        SaveGameManager.CurrentActivityData.FinishTour();
        SaveGameManager.SaveState();
        root.Q<Button>("TourEndButton").clicked += () => SceneManager.LoadScene("NavigationScene");
    }

}
