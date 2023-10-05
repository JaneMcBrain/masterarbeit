using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using SaveLoadSystem;
using UnityEngine.SceneManagement;

public class ObjectChangeUi : MonoBehaviour
{
  private VisualElement uiDocument;

  void Start()
  {
    SaveGameManager.LoadState();
    var currentExercise = SaveGameManager.CurrentActivityData.currentExercise.exercise;
    uiDocument = GetComponent<UIDocument>().rootVisualElement;
    uiDocument.Q<Button>("BackButton").clicked += () => SceneManager.LoadScene("ScanScene");
    var infoToggle = uiDocument.Q<VisualElement>("FooterInfoToggle");
    uiDocument.Q<Label>("TourName").text = currentExercise.imageTitle;
    uiDocument.Q<Label>("TourProgress").text = currentExercise.imageArtist;
    var imagePath = "Sprites/Artwork/" + currentExercise.image;
    uiDocument.Q<VisualElement>("TourImage").style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
    infoToggle.AddManipulator(new Clickable(evt => ToggleInfo(uiDocument)));
  }

  // Update is called once per frame
  void ToggleInfo(VisualElement root)
  {
    var infoText = root.Q<VisualElement>("InteractionTextBox");
    if (infoText.ClassListContains("hidden"))
    {
      infoText.RemoveFromClassList("hidden");
    }
    else
    {
      infoText.AddToClassList("hidden");
    }
  }
}
