using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using SaveLoadSystem;

public class DetectImageTracker : MonoBehaviour
{
    [SerializeField]
    XRReferenceImageLibrary xrReferenceLibrary;
    private ARTrackedImageManager _trackedImageManager;

    [SerializeField]
    public GameObject CorrectImage;
    public GameObject WrongImage;
    public GameObject UI;

    private readonly Dictionary<string, GameObject> _instantiatedArtworks = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, GameObject> _instantiatedFeedback = new Dictionary<string, GameObject>();

    private readonly Dictionary<string, string> interactionText = new Dictionary<string, string>()
    {
        {"ObjectChangeFace", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du Gesichter im Bild austauschen. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."},
        {"ObjectChangeSticker", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du Elemente im Bild austauschen. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."},
        {"ObjectChangePlane", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du Elemente im Bild austauschen. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."},
        {"Cutting", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du Elemente im Bild auschneiden. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."},
        {"ChangeStyle", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du den Stil des Bildes verändern. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."}
    };

    void Start(){
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
        _trackedImageManager.referenceLibrary = _trackedImageManager.CreateRuntimeLibrary(xrReferenceLibrary);
        _trackedImageManager.maxNumberOfMovingImages = 3;
        _trackedImageManager.enabled = true;
        _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnEnable() => _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

    private void OnDisable() => _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs){
        SaveGameManager.LoadState();
        var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;

        foreach(var trackedImage in eventArgs.updated){
            var trackImageName = trackedImage.referenceImage.name;
            //check if trackedImages is tracked && is the correct artwork
            if (trackedImage.trackingState == TrackingState.Tracking && trackImageName == searchedImage)
            {
                //if artwork is correct
                var keyRight = "right_" + trackImageName;
                if (!_instantiatedFeedback.ContainsKey(keyRight)){
                    var newArObj = Instantiate(CorrectImage, trackedImage.transform);
                    newArObj.transform.localScale = new Vector3(trackedImage.referenceImage.size.x, trackedImage.transform.localScale.y, trackedImage.referenceImage.size.y);
                    _instantiatedFeedback[keyRight] = newArObj;
                    _instantiatedFeedback[keyRight].SetActive(true);
                    //Add image content to UI
                    loadInteractionUI();
                }
            }
            if (trackedImage.trackingState == TrackingState.Tracking && trackImageName != searchedImage)
            {
                var keyWrong = "wrong_" + trackImageName;
                if (!_instantiatedFeedback.ContainsKey(keyWrong))
                {
                    var newArObj = Instantiate(WrongImage, trackedImage.transform);
                    _instantiatedFeedback[keyWrong] = newArObj;
                    _instantiatedFeedback[keyWrong].SetActive(true);
                }
            }
        }
        //reset everything on remove
        foreach (var trackedImage in eventArgs.removed)
        {
            Destroy(_instantiatedArtworks[trackedImage.referenceImage.name]);
            Destroy(_instantiatedFeedback["right_" + trackedImage.referenceImage.name]);
            _instantiatedArtworks.Remove(trackedImage.referenceImage.name);
            _instantiatedFeedback.Remove("right_" + trackedImage.referenceImage.name);
        }
    }

    private void loadInteractionUI(){
        var overlay = UI.GetComponent<UIDocument>().rootVisualElement;
        var currentExercise = SaveGameManager.CurrentActivityData.currentExercise.exercise;
        var exType = currentExercise.type;
        Debug.Log(currentExercise);
        //Hide Help Button
        overlay.Q<VisualElement>("FooterHelpToggle").AddToClassList("hidden");
        //Show Image Info
        overlay.Q<VisualElement>("ImageInfo").RemoveFromClassList("hidden");
        //Show Interaction Info and Info Text
        overlay.Q<VisualElement>("InteractionTextBox").RemoveFromClassList("hidden");
        overlay.Q<VisualElement>("FooterInfoToggle").RemoveFromClassList("hidden");
        overlay.Q<Label>("ImageTitle").text = SaveGameManager.CurrentActivityData.currentExercise.exercise.imageTitle;
        overlay.Q<Label>("ImageArtist").text = SaveGameManager.CurrentActivityData.currentExercise.exercise.imageArtist;
        overlay.Q<Label>("InteractionText").text = interactionText[exType];
        overlay.Q<Button>("InteractionStart").clicked += () => LoadInteraction(exType);
        //Here we need to change th currentExercise to ensure the right next Exercise
        overlay.Q<Button>("NextImage").clicked += () => SceneManager.LoadScene("InteractionNavi"); ;
    }

    private void LoadInteraction(string type){
        if (type.Length > 0)
        {
            SceneManager.LoadScene(type + "Scene");
        }
    }
}
