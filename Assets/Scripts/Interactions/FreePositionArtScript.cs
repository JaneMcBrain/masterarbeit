using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using SaveLoadSystem;
using UnityEngine.UIElements;
using Lean;
using Lean.Touch;
using UnityEngine.InputSystem.EnhancedTouch;
using Unity.VisualScripting;

public class FreePositionArtScript : MonoBehaviour
{
    [SerializeField]
    private Sprite[] prefabs;

    [SerializeField]
    XRReferenceImageLibrary xrReferenceLibrary;
    private ARTrackedImageManager _imageManager;
    private readonly Dictionary<string, ARTrackedImage> _instantiatedArtworks = new Dictionary<string, ARTrackedImage>();

    public GameObject UI;
    private VisualElement uiDocument;
    private int currentPrefab = 0;
    private GameObject currentObject;
    private VisualElement assetImage;
    private Label assetName;
    void Start()
    {
        //instantiate xrTrackedImageManager runtime
        _imageManager = GetComponent<ARTrackedImageManager>();
        _imageManager.referenceLibrary = _imageManager.CreateRuntimeLibrary(xrReferenceLibrary);
        _imageManager.enabled = true;
        _imageManager.trackedImagesChanged += OnTrackedImagesChanged;
        //UI
        uiDocument = UI.GetComponent<UIDocument>().rootVisualElement;
        uiDocument.Q<VisualElement>("AssetNavigation").RemoveFromClassList("hidden");
        var btnLeft = uiDocument.Q<Button>("AssetLeftClick");
        var btnRight = uiDocument.Q<Button>("AssetRightClick");
        btnLeft.clicked += () => onLeftBtnClick();
        btnRight.clicked += () => onRightBtnClick();
        assetImage = uiDocument.Q<VisualElement>("AssetImage");
        uiDocument.Q<Label>("AssetName").AddToClassList("hidden");
        setThumbnail();
    }

    void setThumbnail()
    {
        assetImage.style.backgroundImage = new StyleBackground(prefabs[currentPrefab]);
    }

    void OnDisable()
    {
        Lean.Touch.LeanTouch.OnFingerTap -= HandleFingerTap;
    }

    void HandleFingerTap(Lean.Touch.LeanFinger finger)
    {
        Debug.Log($"YOLO Tap on: {finger.StartScreenPosition}");
        var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;
        if (_instantiatedArtworks.ContainsKey(searchedImage) && finger.StartScreenPosition.y > 360)
        {
            if (currentObject != null)
            {
                fixObjectOnScreen();
                setObjectOnScreen();
            }
            else
            {
                setObjectOnScreen();
            }
        }
    }

    void setObjectOnScreen()
    {
        var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;
        // GameObject erstellen
        GameObject spriteObject = new GameObject("SpriteObject");
        // SpriteRenderer-Komponente hinzufügen
        SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
        spriteRenderer.sprite = prefabs[currentPrefab];
        currentObject = Instantiate(spriteObject, _instantiatedArtworks[searchedImage].transform);
        // GameObject um 90 Grad nach unten kippen
        currentObject.transform.Rotate(Vector3.right, 90f);
        currentObject.SetActive(true);
        currentObject.AddComponent<LeanPinchScale>();
        currentObject.AddComponent<LeanDragTranslate>();
        currentObject.AddComponent<LeanTwistRotate>();
        currentObject.AddComponent<LeanTwistRotateAxis>();
    }

    void fixObjectOnScreen()
    {
        var scale = currentObject.GetComponent<LeanPinchScale>();
        var translate = currentObject.GetComponent<LeanDragTranslate>();
        var rotate = currentObject.GetComponent<LeanTwistRotate>();
        var rotateAxis = currentObject.GetComponent<LeanTwistRotateAxis>();
        Destroy(scale);
        Destroy(translate);
        Destroy(rotate);
        Destroy(rotateAxis);
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
                    Lean.Touch.LeanTouch.OnFingerTap += HandleFingerTap;
                }

            }
        }
    }

    private void onRightBtnClick()
    {
        Debug.Log("YOLO : Right Click");
        if (currentPrefab + 1 == prefabs.Length)
        {
            currentPrefab = 0;
        }
        else
        {
            currentPrefab += 1;
        }
        setThumbnail();
    }
    private void onLeftBtnClick()
    {
        Debug.Log("YOLO : Left Click");
        if (currentPrefab == 0)
        {
            currentPrefab = prefabs.Length - 1;
        }
        else
        {
            currentPrefab -= 1;
        }
        setThumbnail();
    }


}
