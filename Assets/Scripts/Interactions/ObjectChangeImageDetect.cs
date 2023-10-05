using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UIElements;


public class ObjectChangeImageDetect : MonoBehaviour
{

    [SerializeField]
    XRReferenceImageLibrary xrReferenceLibrary;
    private ARTrackedImageManager _imageManager;

    [SerializeField]
    public Sprite[] images;
    public GameObject StickerPrefab;
    public GameObject WhiteRect;
    public GameObject UI;
    private VisualElement uiDocument;
    private int objectIndex;
    private int currentSticker = 0;
    private VisualElement currentTrackedImage;

    private readonly Dictionary<string, GameObject> _instantiatedSticker = new Dictionary<string, GameObject>();
    void Start()
    {
        //instantiate xrTrackedImageManager runtime
        _imageManager = GetComponent<ARTrackedImageManager>();
        _imageManager.referenceLibrary = _imageManager.CreateRuntimeLibrary(xrReferenceLibrary);
        _imageManager.maxNumberOfMovingImages = 7;
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
        //this is to add the artwork on top
        foreach (ARTrackedImage trackedImage in eventArgs.added){
            if (trackedImage.trackingState == TrackingState.Tracking){
                var artworkName = trackedImage.referenceImage.name;
                //check if image already has sticker
                if (!_instantiatedSticker.ContainsKey(artworkName))
                {
                    Debug.Log(_instantiatedSticker.Count);
                    if(_instantiatedSticker.Count == 0){
                        Debug.Log("Instanziiere das extra UI");
                        uiDocument = UI.GetComponent<UIDocument>().rootVisualElement;
                        currentTrackedImage = uiDocument.Q<VisualElement>("TrackedImage");
                        var SelectImagePanel = uiDocument.Q<VisualElement>("TrackedImageSelect");
                        setTrackedImageToUI(artworkName);
                        SelectImagePanel.RemoveFromClassList("hidden");
                        var changeImageBtn = uiDocument.Q<Button>("ChangeImageBtn");
                        changeImageBtn.clicked += () => onChangeImageClick();
                    }
                    // Give the initial image a reasonable default scale
                    var minLocalScalar = Mathf.Min(trackedImage.size.x, trackedImage.size.y) / 2;
                    trackedImage.transform.localScale = new Vector3(minLocalScalar, minLocalScalar, minLocalScalar);
                    var sticker = Instantiate(WhiteRect, trackedImage.transform);
                    Debug.Log("TRACKED IMAGE YOLO: " + artworkName);
                    _instantiatedSticker[artworkName] = sticker;
                    _instantiatedSticker[artworkName].SetActive(true);
                }
            }
        }
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            // Give the initial image a reasonable default scale
            var minLocalScalar = Mathf.Min(trackedImage.size.x, trackedImage.size.y);
            trackedImage.transform.localScale = new Vector3(minLocalScalar, minLocalScalar, minLocalScalar);
        }
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
