using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using SaveLoadSystem;
using UnityEngine.UIElements;

public class ChangeStyleScript : MonoBehaviour
{
    [SerializeField]
    private Sprite[] artworks;

    public GameObject artObject;

    [SerializeField]
    XRReferenceImageLibrary xrReferenceLibrary;
    private ARTrackedImageManager _imageManager;
    private readonly Dictionary<string, ARTrackedImage> _instantiatedArtworks = new Dictionary<string, ARTrackedImage>();

    public GameObject UI;
    private VisualElement uiDocument;

    private GameObject instantiatedOverlay;

    void Start()
    {
        //instantiate xrTrackedImageManager runtime
        _imageManager = GetComponent<ARTrackedImageManager>();
        _imageManager.referenceLibrary = _imageManager.CreateRuntimeLibrary(xrReferenceLibrary);
        _imageManager.enabled = true;
        _imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        //UI
        uiDocument = UI.GetComponent<UIDocument>().rootVisualElement;
        uiDocument.Q<VisualElement>("ChangeObjectButtons").RemoveFromClassList("hidden");
        for(int i = 0; i < artworks.Length; i++){
            Button button = uiDocument.Q<Button>("ChangeObject" + (i+1));
            button.text = artworks[i].name;
            Debug.Log($"YOLO changeStyle: {i}");
            int index = i;
            button.clicked += () => changeStyle(index, button);
        }
        Button resetBtn = uiDocument.Q<Button>("ChangeObjectReset");
        resetBtn.clicked += () => resetImage(resetBtn);
    }


    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        SaveGameManager.LoadState();
        var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;
        foreach (var trackedImage in eventArgs.updated)
        {
            var artworkName = trackedImage.referenceImage.name;
            if (trackedImage.trackingState == TrackingState.Tracking && artworkName == searchedImage)
            {
                if (!_instantiatedArtworks.ContainsKey(artworkName))
                {
                    _instantiatedArtworks[artworkName] = trackedImage;
                    instantiatedOverlay = Instantiate(artObject, trackedImage.transform);
                    instantiatedOverlay.transform.localScale = new Vector3(trackedImage.referenceImage.size.x, trackedImage.referenceImage.size.y, trackedImage.transform.localScale.z);
                    instantiatedOverlay.SetActive(true);
                }
            }
        }
    }

    void changeStyle(int index, Button btn)
    {
        resetActiveBtn();
        SpriteRenderer sr = instantiatedOverlay.GetComponent<SpriteRenderer>();
        Sprite sprite = artworks[index];
        sr.sprite = sprite;
        sr.color = new Color(1, 1, 1, 1);
        btn.AddToClassList("is-active");
    }
    void resetImage(Button btn)
    {
        resetActiveBtn();
        SpriteRenderer sr = instantiatedOverlay.GetComponent<SpriteRenderer>();
        sr.sprite = null;
        sr.color = new Color(1, 1, 1, 0);
        btn.AddToClassList("is-active");
    }

    void resetActiveBtn(){
        var activeButton = uiDocument.Q<Button>(className: "is-active");
        if (activeButton != null)
        {
            activeButton.RemoveFromClassList("is-active");
        }
    }


}
