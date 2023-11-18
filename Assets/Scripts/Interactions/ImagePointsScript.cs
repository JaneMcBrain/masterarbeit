using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UIElements;
using SaveLoadSystem;
using System.Security.Cryptography;


public class ImagePointsScript : MonoBehaviour
{

    [SerializeField]
    XRReferenceImageLibrary xrReferenceLibrary;
    private ARTrackedImageManager _imageManager;

    [SerializeField]
    public Sprite[] images;
    public GameObject StickerPrefab;
    public GameObject WhiteRect;
    public GameObject YellowRect;
    public GameObject UI;
    private VisualElement uiDocument;
    private int objectIndex = 0;
    private int currentSticker = 0;
    private VisualElement currentTrackedImage;

    private readonly Dictionary<string, GameObject> _instantiatedSticker = new Dictionary<string, GameObject>();

    List<Vector3> positions = new List<Vector3>
        {
            new Vector3(0f, 0f, 0f),
            new Vector3(-0.5f, -0.28f, 0f)
        };
    List<Vector3> sizes = new List<Vector3>
        {
            new Vector3(0.01f, 0.01f, 0.01f),
            new Vector3(0.05f, 0.05f, 0.05f)
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
        var changeBtn1 = uiDocument.Q<Button>("ChangeObject1");
        var changeBtn2 = uiDocument.Q<Button>("ChangeObject2"); 
        var changeBtn3 = uiDocument.Q<Button>("ChangeObject3");
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
                    addWhiteRect(trackedImage.transform.position + positions[i], trackedImage.transform.localScale / (i+2), Quaternion.identity, key, i);
                }
            }
        }
    }

    public void addWhiteRect(Vector3 position, Vector3 size, Quaternion rotation, string key, int index)
    {
        GameObject sticker;
        if (objectIndex == index){
            sticker = Instantiate(WhiteRect, position, rotation);
        } else {
            sticker = Instantiate(YellowRect, position, rotation);
        }
        sticker.transform.localScale = size;
        _instantiatedSticker[key] = sticker;
        _instantiatedSticker[key].SetActive(true);

        Debug.Log($"Image: {key} is at " +
                    $"{sticker.transform.position} and has scale {sticker.transform.localScale}");

    }

    void setTrackedImageToUI(string objectName)
    {
        var imagePath = "Sprites/Artwork/" + objectName;
        currentTrackedImage.style.backgroundImage = new StyleBackground(Resources.Load<Sprite>(imagePath));
    }

    void onChangeImageClick(){
        var objectName = getObjectName();
        setTrackedImageToUI(objectName);
        if (currentSticker + 1 == _instantiatedSticker.Count)
        {
            currentSticker = 0;
        } else {
            currentSticker += 1;
        }
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
