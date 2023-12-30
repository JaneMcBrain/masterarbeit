using System.Collections;
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

    Vector2 imageSize = new Vector2(2560, 2012);
    List<Vector3> positions = new List<Vector3>
        {
            new Vector3(1375, -1467, 0),
            new Vector3(164, -1229, 0),
            new Vector3(1830, -1453, 0)
        };
    List<Vector3> sizes = new List<Vector3>
        {
            new Vector3(112, 112, 0),
            new Vector3(92, 92, 0),
            new Vector3(217, 482, 0)
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

    void setGameObjectParams(GameObject currentObject, ARTrackedImage image, Vector3 position, Vector3 size)
    {   //Größe
        Vector2 multiplier = new Vector2(image.size.x / imageSize.x, image.size.y / imageSize.y);
        float recWidth = multiplier.x * size.x;
        float recHeight = multiplier.y * size.y;
        currentObject.transform.localScale = new Vector3(recWidth, recHeight, 1f);
        //Position
        Vector3 pos = image.transform.position - new Vector3(image.size.x / 2, -image.size.y / 2, 0);
        Vector3 faceCoordinates = new Vector3(position.x * multiplier.x + recWidth, position.y * multiplier.y - recHeight, 0);
        currentObject.transform.position = pos + faceCoordinates;

        Debug.Log($"YOLO Berechnung {image.transform.position} - {new Vector3(image.size.x / 2, image.size.y / 2, 0)}");
        Debug.Log($"YOLO Berechnung {new Vector3(image.size.x, image.size.y, 0)}");
        Debug.Log($"YOLO faceCoordinates: {faceCoordinates}");
        Debug.Log($"YOLO Position: {pos}");
        Debug.Log($"YOLO instantiatedRects Position: {currentObject.transform.position}");
        Debug.Log($"YOLO multiplier: {multiplier.ToString("F6")}");
        Debug.Log($"YOLO neue Berechnung: {pos.ToString("F6")} + {faceCoordinates.ToString("F6")}");
        Debug.Log($"YOLO Image Position: {image.transform.position}");
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
        Debug.Log($"YOLO onChangeObject objectName: {objectName}");
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
