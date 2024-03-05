using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UIElements;
using SaveLoadSystem;
using System;


public class ImagePointsScript : MonoBehaviour
{
    [SerializeField]
    XRReferenceImageLibrary xrReferenceLibrary;
    private ARTrackedImageManager _imageManager;

    [SerializeField]
    public Sprite[] prefabs;
    public GameObject WhiteRect;
    public GameObject HighlightRect;
    public GameObject UI;
    private VisualElement uiDocument;
    private int currentObjectIndex = 0;
    private readonly Dictionary<string, GameObject> _instantiatedSticker = new Dictionary<string, GameObject>();

    [SerializeField]
    public Vector2 imagePixelSize;

    [SerializeField]
    List<Vector2> positions;

    [SerializeField]
    List<Vector2> sizes;

    private int currentStickerIndex = 0;
    private VisualElement assetImage;
    private Label assetName;

    void Start()
    {
        //instantiate xrTrackedImageManager runtime
        _imageManager = GetComponent<ARTrackedImageManager>();
        _imageManager.referenceLibrary = _imageManager.CreateRuntimeLibrary(xrReferenceLibrary);
        _imageManager.enabled = true;
        _imageManager.trackedImagesChanged += OnTrackedImagesChanged;

        uiDocument = UI.GetComponent<UIDocument>().rootVisualElement;
        //show UI elements
        uiDocument.Q<VisualElement>("AssetNavigation").RemoveFromClassList("hidden");
        uiDocument.Q<VisualElement>("PositionNavigation").RemoveFromClassList("hidden");
        //get Buttons
        var btnLeft = uiDocument.Q<Button>("AssetLeftClick");
        var btnRight = uiDocument.Q<Button>("AssetRightClick");
        var switchPlus = uiDocument.Q<Button>("PositionSelectPlus");
        var switchMinus = uiDocument.Q<Button>("PositionSelectMinus");
        var applyBtn = uiDocument.Q<Button>("ApplyButton");
        //add onClick event handler
        switchPlus.clicked += () => onSwitchPosition(1);
        switchMinus.clicked += () => onSwitchPosition(-1);
        applyBtn.clicked += () => setSticker();
        btnLeft.clicked += () => onLeftBtnClick();
        btnRight.clicked += () => onRightBtnClick();

        assetImage = uiDocument.Q<VisualElement>("AssetImage");
        assetName = uiDocument.Q<Label>("AssetName");
        setThumbnail();
    }

    void setThumbnail()
    {

        assetName.text = prefabs[currentStickerIndex].name;
        assetImage.style.backgroundImage = new StyleBackground(prefabs[currentStickerIndex]);
    }

    private void OnDisable() => _imageManager.trackedImagesChanged -= OnTrackedImagesChanged;

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        SaveGameManager.LoadState();
        var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;

        foreach (var trackedImage in eventArgs.updated)
        {
            var artworkName = trackedImage.referenceImage.name;
            //check if trackedImages is tracked && is the correct artwork
            if (trackedImage.trackingState == TrackingState.Tracking && artworkName == searchedImage)
            {
                for (int i = 0; i < positions.Count; i++)
                {
                    var key = $"{artworkName}_{i}";
                    if (!_instantiatedSticker.ContainsKey(key)){
                        addRectPointer(trackedImage, key, i);
                    } else {
                        setGameObjectParams(_instantiatedSticker[key], trackedImage, positions[i], sizes[i]);
                    }

                }
            }
        }
    }

    public void addRectPointer(ARTrackedImage image, string key, int index)
    {
        GameObject sticker;
        Transform newTransform = image.transform;
        if (currentObjectIndex != index){
            sticker = Instantiate(WhiteRect, newTransform);
        } else {
            sticker = Instantiate(HighlightRect, newTransform);
        }
        setGameObjectParams(sticker, image, positions[index], sizes[index]);
        _instantiatedSticker[key] = sticker;
        _instantiatedSticker[key].SetActive(true);
    }

    void setGameObjectParams(GameObject currentObject, ARTrackedImage image, Vector2 position, Vector2 size)
    {
        float imageZScale = image.transform.localScale.z;
        //Größe
        // Unity world size / pixel = multiplier für Pixelumrechnung in Unity scala
        Vector2 multiplier = new Vector2(image.size.x / imagePixelSize.x, image.size.y / imagePixelSize.y);
        float recWidth = multiplier.x * size.x;
        float recHeight = multiplier.y * size.y;
        // hier z neu berechnen?
        currentObject.transform.localScale = new Vector3(recWidth, recHeight, imageZScale);
        //Position
        // Verschieben der Image-Position nach oben links, als Ausgangspunkt 0
        Vector3 posZero = image.transform.position - new Vector3(image.size.x / 2, -image.size.y / 2, 0);
        //Umrechnung der Gesichtskoordinaten von Pixel zu Unity scale minus/plus der halben Rectangle Größe 
        Vector3 faceCoordinates = new Vector3(position.x * multiplier.x + (recWidth/2), position.y * multiplier.y - (recHeight/2), 0);
        currentObject.transform.position = posZero + faceCoordinates;
    }

    void onSwitchPosition(int step){
        string stickerIndex = getObjectName();
        float alpha = _instantiatedSticker[stickerIndex].GetComponent<SpriteRenderer>().sprite.name.Contains("Square") ? 0.5f : 0.8f;
        _instantiatedSticker[stickerIndex].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
        int newIndex = currentObjectIndex + step;
        if(newIndex == _instantiatedSticker.Count){
            currentObjectIndex = 0;
        } else if(newIndex == -1) {
            currentObjectIndex = _instantiatedSticker.Count - 1;
        } else {
            currentObjectIndex = newIndex;
        }
        alpha = _instantiatedSticker[stickerIndex].GetComponent<SpriteRenderer>().sprite.name.Contains("Square") ? 0.5f : 0.8f;
        _instantiatedSticker[getObjectName()].GetComponent<SpriteRenderer>().color = new Color(0.18f, 0.64f, 0.94f, alpha);
    }

    string getObjectName(){
        List<string> stickerKeys = new List<string>(_instantiatedSticker.Keys);
        var currentKey = stickerKeys[currentObjectIndex];
        return currentKey;
    }

    void setSticker()
    {
        var objectName = getObjectName();
        _instantiatedSticker[objectName].GetComponent<SpriteRenderer>().sprite = prefabs[currentStickerIndex];
        _instantiatedSticker[objectName].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.8f);
    }

    private void onRightBtnClick()
    {
        if (currentStickerIndex + 1 == prefabs.Length)
        {
            currentStickerIndex = 0;
        }
        else
        {
            currentStickerIndex += 1;
        }
        setThumbnail();
    }
    private void onLeftBtnClick()
    {
        if (currentStickerIndex == 0)
        {
            currentStickerIndex = prefabs.Length - 1;
        }
        else
        {
            currentStickerIndex -= 1;
        }
        setThumbnail();
    }
}
