using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using SaveLoadSystem;
using UnityEngine.UIElements;
using Lean.Touch;

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

    private bool FingerIsOnImage(Lean.Touch.LeanFinger finger){
        return finger.StartScreenPosition.y > 580 && finger.StartScreenPosition.y < 2300;
    }

    void HandleFingerTap(Lean.Touch.LeanFinger finger)
    {
        var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;
        //check if image correct tracked, finger on the image area
        if (_instantiatedArtworks.ContainsKey(searchedImage) && FingerIsOnImage(finger))
        {
            //check if currentObject is not null and alredy used
            if(currentObject != null && currentObject.name != prefabs[currentPrefab].name){
                return;
            } else {
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
        Vector3 currentScale = transform.localScale;
        currentObject.transform.localScale = new Vector3(currentScale.x * 0.3f, currentScale.y * 0.3f, currentScale.z);
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
