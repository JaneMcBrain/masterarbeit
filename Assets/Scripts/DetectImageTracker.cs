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
        {"FaceFilter", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du Gesichter im Bild austauschen. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."},
        {"ImagePointMeme", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du Memes und Comigfiguren in das Bild setzen. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."},
        {"ImagePointCelebrity", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du Celebrities in das Bild setzen. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."},
        {"FreePosition", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du neue Tiere in das Bild packen. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."},
        {"FreePositionArt", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du erste Skizzen des Bildes auf das Bild legen. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."},
        {"ChangeStyle", "Du hast eine Interaktion freigeschaltet! Bei diesem Aufgabentyp, kannst du den Stil des Bildes verändern. Klicke 'Start' um fortzufahren oder such das nächste Bild, indem du auf 'Nächstes Bild' klickst."}
    };

    void Start(){
        _trackedImageManager = GetComponent<ARTrackedImageManager>();
        _trackedImageManager.referenceLibrary = _trackedImageManager.CreateRuntimeLibrary(xrReferenceLibrary);
        _trackedImageManager.enabled = true;
        _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnEnable() => _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

    private void OnDisable() => _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs){
        SaveGameManager.LoadState();
        var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;
        Debug.Log($"YOLO searchedImage: " + SaveGameManager.CurrentActivityData.currentExercise.exercise.image);
        foreach(var trackedImage in eventArgs.updated){
            var trackImageName = trackedImage.referenceImage.name;
            //check if trackedImages is tracked && is the correct artwork
            if (trackedImage.trackingState == TrackingState.Tracking && trackImageName == searchedImage)
            {
                //if artwork is correct
                var keyRight = "right_" + trackImageName;
                if (!_instantiatedFeedback.ContainsKey(keyRight)){
                    setOverlay(CorrectImage, trackedImage, keyRight);
                    //Add image content to UI
                    loadInteractionUI();
                }
            }
            if (trackedImage.trackingState == TrackingState.Tracking && trackImageName != searchedImage)
            {
                var keyWrong = "wrong_" + trackImageName;
                if (!_instantiatedFeedback.ContainsKey(keyWrong))
                {
                    setOverlay(WrongImage, trackedImage, keyWrong);
                }
            }
        }
        //reset everything on remove
        foreach (var trackedImage in eventArgs.removed)
        {
            Destroy(_instantiatedFeedback["right_" + trackedImage.referenceImage.name]);
            _instantiatedFeedback.Remove("right_" + trackedImage.referenceImage.name);
        }
    }

    private void setOverlay(GameObject overlay, ARTrackedImage image, string key){
        var instantiatedOverlay = Instantiate(overlay, image.transform);
        instantiatedOverlay.transform.localScale = new Vector3(image.referenceImage.size.x, image.referenceImage.size.y, image.transform.localScale.z);
        _instantiatedFeedback[key] = instantiatedOverlay;
        _instantiatedFeedback[key].SetActive(true);
    }

    private void loadInteractionUI(){
        var overlay = UI.GetComponent<UIDocument>().rootVisualElement;
        var currentExercise = SaveGameManager.CurrentActivityData.currentExercise.exercise;
        var exType = currentExercise.type;
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
        Debug.Log($"YOLO InteractionStart with type: {exType}");
        overlay.Q<Button>("InteractionStart").clicked += () => LoadInteraction(exType);
        //Here we need to change th currentExercise to ensure the right next Exercise
        overlay.Q<Button>("NextImage").clicked += () => nextExercise();
    }

    private void nextExercise(){
        SaveGameManager.CurrentActivityData.FinishExercise();
        SaveGameManager.SaveState();
        SceneManager.LoadScene("InteractionNavi");
    }

    private void LoadInteraction(string type){
        if (type.Length > 0)
        {
            SceneManager.LoadScene(type + "Scene");
        }
    }
}
