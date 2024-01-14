using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UIElements;
using SaveLoadSystem;


public class ImagePointsScript : MonoBehaviour
{

    [SerializeField]
    XRReferenceImageLibrary xrReferenceLibrary;
    private ARTrackedImageManager _imageManager;

    [SerializeField]
    public Sprite[] images;
    public GameObject WhiteRect;
    public GameObject HighlightRect;
    public GameObject UI;
    private VisualElement uiDocument;
    private int objectIndex = 0;
    private int currentSticker = 0;

    private readonly Dictionary<string, GameObject> _instantiatedSticker = new Dictionary<string, GameObject>();

    Vector2 imagePixelSize = new Vector2(2560, 2012);
    List<Vector3> positions = new List<Vector3>
        {
            new Vector2(1375, -1467),
            new Vector2(164, -1229),
            new Vector2(2307, -1358),
            new Vector2(1981, -1146)
        };
    List<Vector3> sizes = new List<Vector3>
        {
            new Vector2(112, 112),
            new Vector2(92, 92),
            new Vector2(86, 86),
            new Vector2(74, 74)
        };

    void Start()
    {
        //instantiate xrTrackedImageManager runtime
        _imageManager = GetComponent<ARTrackedImageManager>();
        _imageManager.referenceLibrary = _imageManager.CreateRuntimeLibrary(xrReferenceLibrary);
        _imageManager.enabled = true;
        _imageManager.trackedImagesChanged += OnTrackedImagesChanged;

        uiDocument = UI.GetComponent<UIDocument>().rootVisualElement;
        uiDocument.Q<VisualElement>("ChangeObjectButtons").RemoveFromClassList("hidden");
        var switchBtn = uiDocument.Q<Button>("TrackedPositionSelect");
        switchBtn.RemoveFromClassList("hidden");
        var changeBtn1 = uiDocument.Q<Button>("ChangeObject1");
        var changeBtn2 = uiDocument.Q<Button>("ChangeObject2"); 
        var changeBtn3 = uiDocument.Q<Button>("ChangeObject3");
        switchBtn.clicked += () => onSwitchPosition();
        changeBtn1.clicked += () => onChangeObject(changeBtn1, 0);
        changeBtn2.clicked += () => onChangeObject(changeBtn2, 1);
        changeBtn3.clicked += () => onChangeObject(changeBtn3, 2);
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
        if (objectIndex != index){
            sticker = Instantiate(WhiteRect, newTransform);
        } else {
            sticker = Instantiate(HighlightRect, newTransform);
        }
        setGameObjectParams(sticker, image, positions[index], sizes[index]);
        _instantiatedSticker[key] = sticker;
        _instantiatedSticker[key].SetActive(true);
    }

    void setGameObjectParams(GameObject currentObject, ARTrackedImage image, Vector2 position, Vector2 size)
    {   float imageZPos = image.transform.position.z;
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

    void onSwitchPosition(){
        float alpha = 1.0f;
        string stickerIndex = getObjectName();
        if (_instantiatedSticker[stickerIndex].GetComponent<SpriteRenderer>().sprite.name.Contains("Rect")){
            alpha = 0.5f;
        }
        _instantiatedSticker[stickerIndex].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
        if (currentSticker + 1 == _instantiatedSticker.Count)
        {
            currentSticker = 0;
        } else {
            currentSticker += 1;
        }
        _instantiatedSticker[getObjectName()].GetComponent<SpriteRenderer>().color = new Color(0.18f, 0.64f, 0.94f, 0.5f);
    }

    string getObjectName(){
        List<string> stickerKeys = new List<string>(_instantiatedSticker.Keys);
        var currentKey = stickerKeys[currentSticker];
        return currentKey;
    }

    void onChangeObject(VisualElement btn, int index)
    {
        objectIndex = index;
        activateButton(btn);
        var objectName = getObjectName();
        _instantiatedSticker[objectName].GetComponent<SpriteRenderer>().sprite = images[objectIndex];
        _instantiatedSticker[objectName].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    void activateButton(VisualElement btn)
    {
        VisualElement result = uiDocument.Q(className: "is-active");
        if (result != null)
        {
            result.RemoveFromClassList("is-active");
        }
        btn.AddToClassList("is-active");
    }
}
