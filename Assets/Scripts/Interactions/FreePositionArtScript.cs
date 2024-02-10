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
    private ARTrackedImageManager imageManager;
    private readonly Dictionary<string, ARTrackedImage> instantiatedArtworks = new Dictionary<string, ARTrackedImage>();
    private readonly Dictionary<string, GameObject> instantiatedPrefabs = new Dictionary<string, GameObject>();

    public GameObject UI;
    private VisualElement uiDocument;
    private int currentPrefabIndex = 0;
    private VisualElement assetImage;
    void Start()
    {
        //instantiate xrTrackedImageManager runtime
        imageManager = GetComponent<ARTrackedImageManager>();
        imageManager.referenceLibrary = imageManager.CreateRuntimeLibrary(xrReferenceLibrary);
        imageManager.enabled = true;
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
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
        assetImage.style.backgroundImage = new StyleBackground(prefabs[currentPrefabIndex]);
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
        if (instantiatedArtworks.ContainsKey(searchedImage) && FingerIsOnImage(finger))
        {

            setObjectOnScreen();
        }
    }

    void setObjectOnScreen()
    {
        var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;
        string objName = "prefab_" + currentPrefabIndex;
        if (!instantiatedPrefabs.ContainsKey(objName)){
            // GameObject erstellen
            GameObject spriteObject = new GameObject("SpriteObject");
            // SpriteRenderer-Komponente hinzufügen
            SpriteRenderer spriteRenderer = spriteObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = prefabs[currentPrefabIndex];

            spriteRenderer.size = new Vector2(spriteRenderer.size.x * 0.5f, spriteRenderer.size.y * 0.5f);
            instantiatedPrefabs[objName] = Instantiate(spriteObject, instantiatedArtworks[searchedImage].transform);
            // GameObject um 90 Grad nach unten kippen
            instantiatedPrefabs[objName].transform.Rotate(Vector3.right, 90f);
            instantiatedPrefabs[objName].SetActive(true);
            AddLeanGestures(objName);
        } else {
            AddLeanGestures(objName);
        }
    }

    void AddLeanGestures(string objName){
        instantiatedPrefabs[objName].AddComponent<LeanPinchScale>();
        instantiatedPrefabs[objName].AddComponent<LeanDragTranslate>();
        instantiatedPrefabs[objName].AddComponent<LeanTwistRotate>();
        instantiatedPrefabs[objName].AddComponent<LeanTwistRotateAxis>();
    }

    void fixObjectOnScreen()
    {
        string objName = "prefab_" + currentPrefabIndex;
        if (instantiatedPrefabs.ContainsKey(objName)){
            var scale = instantiatedPrefabs[objName].GetComponent<LeanPinchScale>();
            var translate = instantiatedPrefabs[objName].GetComponent<LeanDragTranslate>();
            var rotate = instantiatedPrefabs[objName].GetComponent<LeanTwistRotate>();
            var rotateAxis = instantiatedPrefabs[objName].GetComponent<LeanTwistRotateAxis>();
            Destroy(scale);
            Destroy(translate);
            Destroy(rotate);
            Destroy(rotateAxis);
        }
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
                if (!instantiatedArtworks.ContainsKey(artworkName))
                {
                    instantiatedArtworks[artworkName] = trackedImage;
                    Lean.Touch.LeanTouch.OnFingerTap += HandleFingerTap;
                }

            }
        }
    }

    private void onRightBtnClick()
    {
        fixObjectOnScreen();
        if (currentPrefabIndex + 1 == prefabs.Length)
        {
            currentPrefabIndex = 0;
        }
        else
        {
            currentPrefabIndex += 1;
        }
        setThumbnail();
    }
    private void onLeftBtnClick()
    {
        fixObjectOnScreen();
        if (currentPrefabIndex == 0)
        {
            currentPrefabIndex = prefabs.Length - 1;
        }
        else
        {
            currentPrefabIndex -= 1;
        }
        setThumbnail();
    }


}
