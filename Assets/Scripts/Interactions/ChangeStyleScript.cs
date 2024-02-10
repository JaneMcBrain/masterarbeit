using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using SaveLoadSystem;
using UnityEngine.UIElements;
using Lean.Touch;

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
        for(int i = 0; i< artworks.Length; i++){
            var button = uiDocument.Q<Button>("ChangeObject" + (i+1));
            button.text = artworks[i].name;
            button.clicked += () => changeStyle(i);
        }
        uiDocument.Q<Button>("ChangeObjectReset").clicked += () => resetImage();
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
                    var instantiatedOverlay = Instantiate(artObject, trackedImage.transform);
                    instantiatedOverlay.transform.localScale = new Vector3(trackedImage.referenceImage.size.x, trackedImage.referenceImage.size.y, trackedImage.transform.localScale.z);
                    instantiatedOverlay.SetActive(true);
                }
            }
        }
    }

    void changeStyle(int index)
    {
        SpriteRenderer sr = artObject.GetComponent<SpriteRenderer>();
        Sprite sprite = artworks[index];
        sr.sprite = sprite;
        sr.color = new Color(1, 1, 1, 1);
    }
    void resetImage()
    {
        SpriteRenderer sr = artObject.GetComponent<SpriteRenderer>();
        sr.sprite = null;
        sr.color = new Color(1, 1, 1, 0);
    }


}
