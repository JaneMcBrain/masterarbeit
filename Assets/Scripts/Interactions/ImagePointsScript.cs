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

    List<Vector3> positions = new List<Vector3>
        {
            new Vector3(0.038f, -0.175f, 0),
            new Vector3(-0.39f, -0.08f, 0),
            new Vector3(0.2136f, -0.1777f, 0)
        };
    List<Vector3> sizes = new List<Vector3>
        {   //x = whiteRec x / Image X
            //y = whiteRec y / Image Y
            //Vector3(whiteRec x / 2560, whiteRec y / 2012)
            new Vector3(0.0289f, 0.0368f, 0),
            new Vector3(0.0345f, 0.0457f, 0),
            new Vector3(0.0847f, 0.239f, 0)
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
    {
        Transform imgTransform = image.transform;
        //calculate size
        Vector3 stickerSize = new Vector3(image.size.x * size.x, image.size.y * size.y, 1f);
        //calculate position
        //image position + position of array +/- half size of image (unity centers positions)
        float posX = imgTransform.position.x + position.x + (stickerSize.x * 0.5f);
        float posY = imgTransform.position.y + position.y - (stickerSize.y * 0.5f);
        Vector3 stickerPosition = new Vector3(posX, posY, imgTransform.position.z);
        //save new values in Game Object
        currentObject.transform.localScale = stickerSize;
        currentObject.transform.position = stickerPosition;
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
