using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UIElements;
using SaveLoadSystem;

public class FaceFilterCelebScript : MonoBehaviour
{
    public Sprite[] sprites;
    public GameObject UI;
    public TextAsset jsonFile;
    private VisualElement uiDocument;
    public GameObject WhiteRect;
    [SerializeField]
    XRReferenceImageLibrary xrReferenceLibrary;


    private ARTrackedImageManager _imageManager;
    private readonly Dictionary<string, ARTrackedImage> _instantiatedArtworks = new Dictionary<string, ARTrackedImage>();
    private readonly Dictionary<string, GameObject> instantiatedRects = new Dictionary<string, GameObject>();

    private AllFacesData allFacesData;
    private string searchedImage;

    private int currentFaceIndex = 0;

    private VisualElement assetImage;
    private Label assetName;
    private int currentPrefabIndex = 0;

    private Vector2 multiplier;

    private Button switchPlus;
    private Button switchMinus;

    void OnEnable()
    {
        //instantiate xrTrackedImageManager runtime
        _imageManager = GetComponent<ARTrackedImageManager>();
        _imageManager.referenceLibrary = _imageManager.CreateRuntimeLibrary(xrReferenceLibrary);
        _imageManager.enabled = true;
        _imageManager.trackedImagesChanged += OnTrackedImagesChanged;

        uiDocument = UI.GetComponent<UIDocument>().rootVisualElement;
        uiDocument.Q<VisualElement>("AssetNavigation").RemoveFromClassList("hidden");
        uiDocument.Q<VisualElement>("PositionNavigation").RemoveFromClassList("hidden");
        //get Buttons
        var btnLeft = uiDocument.Q<Button>("AssetLeftClick");
        var btnRight = uiDocument.Q<Button>("AssetRightClick");
        switchPlus = uiDocument.Q<Button>("PositionSelectPlus");
        switchMinus = uiDocument.Q<Button>("PositionSelectMinus");
        var applyBtn = uiDocument.Q<Button>("ApplyButton");

        if (jsonFile != null)
        {
            // Lade JSON-Datei und deserialisiere sie
            allFacesData = JsonUtility.FromJson<AllFacesData>(jsonFile.text);
        }
        else
        {
            Debug.LogError("All Faces Data JSON not assigned!");
        }

        //add onClick event handler
        if(allFacesData.images.Count > 1){
            switchPlus.clicked += () => onSwitchPosition(1);
            switchMinus.clicked += () => onSwitchPosition(-1);
        } else {
            switchPlus.AddToClassList("hidden");
            switchMinus.AddToClassList("hidden");
        }
        applyBtn.clicked += () => setSticker();
        btnLeft.clicked += () => onLeftBtnClick();
        btnRight.clicked += () => onRightBtnClick();

        assetImage = uiDocument.Q<VisualElement>("AssetImage");
        assetName = uiDocument.Q<Label>("AssetName");
        setThumbnail();
    }

    void setThumbnail()
    {
        assetName.text = sprites[currentPrefabIndex].name;
        assetImage.style.backgroundImage = new StyleBackground(sprites[currentPrefabIndex]);
    }

    void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        SaveGameManager.LoadState();
        searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;

        foreach (var trackedImage in eventArgs.updated)
        {
            var artworkName = trackedImage.referenceImage.name;
            if (trackedImage.trackingState == TrackingState.Tracking && artworkName == searchedImage)
            {
                if (!_instantiatedArtworks.ContainsKey(artworkName))
                {
                    _instantiatedArtworks[artworkName] = trackedImage;
                    highlightFaces(trackedImage);
                }

            }
        }
    }

    void highlightFaces(ARTrackedImage trackedImage)
    {
        // Lade JSON-Datei und deserialisiere sie
        AllFacesData allFacesData = JsonUtility.FromJson<AllFacesData>(jsonFile.text);
        // Iteriere über alle Bilder
        foreach (FaceImage faceImage in allFacesData.images)
        {
            string imagePath = faceImage.imagePath;
            Vector2 imageSize = faceImage.size;
            SaveGameManager.LoadState();
            var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;
            if (imagePath == searchedImage)
            {
                List<RectangleData> faces = faceImage.faceData.faces;
                for (int i = 0; i < faces.Count; i++)
                {
                    RectangleData face = faces[i];
                    string key = imagePath + "_face_" + i;
                    Vector2 multiplier = new Vector2(trackedImage.size.x / imageSize.x, trackedImage.size.y / imageSize.y);
                    Transform imgTransform = trackedImage.transform;
                    if (!instantiatedRects.ContainsKey(key))
                    {
                        //Berechnung der Weltgröße je Pixel
                        GameObject rectangle;
                        rectangle = Instantiate(WhiteRect, imgTransform);
                        // Rechteckgröße neu berechnen
                        float recWidth = multiplier.x * face.width;
                        float recHeight = multiplier.y * face.height;
                        rectangle.transform.localScale = new Vector3(recWidth, recHeight, imgTransform.position.z);
                        // Rechteckposition neu berechnen
                        // 1. Rechteck am Bild unten links positionen
                        setObjectPosition(trackedImage, multiplier, face, rectangle);
                        instantiatedRects[key] = rectangle;
                        instantiatedRects[key].SetActive(true);
                    }
                    else
                    {
                        setObjectPosition(trackedImage, multiplier, face, instantiatedRects[key]);
                    }
                }
            }
        }
    }

    void setObjectPosition(ARTrackedImage img, Vector2 multiplier, RectangleData face, GameObject gameO)
    {
        Debug.Log($"YOLO Position: {gameO.transform.position}");
        Vector3 pos = img.transform.position - new Vector3(img.size.x / 2, -img.size.y / 2, 0);
        Vector3 faceCoordinates = new Vector3(face.x * multiplier.x, -(face.y * multiplier.y), 0);
        gameO.transform.position = pos + faceCoordinates;
    }

    void onSwitchPosition(int step)
    {
        string stickerIndex = getObjectName();
        float alpha = instantiatedRects[stickerIndex].GetComponent<SpriteRenderer>().sprite.name.Contains("Square") ? 0.5f : 0.8f;
        instantiatedRects[stickerIndex].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
        int newIndex = currentFaceIndex + step;
        if (newIndex == instantiatedRects.Count)
        {
            currentFaceIndex = 0;
        }
        else if (newIndex == -1)
        {
            currentFaceIndex = instantiatedRects.Count - 1;
        }
        else
        {
            currentFaceIndex = newIndex;
        }
        alpha = instantiatedRects[stickerIndex].GetComponent<SpriteRenderer>().sprite.name.Contains("Square") ? 0.5f : 0.8f;
        instantiatedRects[getObjectName()].GetComponent<SpriteRenderer>().color = new Color(0.18f, 0.64f, 0.94f, alpha);
    }

    string getObjectName()
    {
        List<string> stickerKeys = new List<string>(instantiatedRects.Keys);
        var currentKey = stickerKeys[currentFaceIndex];
        return currentKey;
    }

    void setSticker()
    {
        var objectName = getObjectName();
        GameObject go = instantiatedRects[objectName];
        SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
        Sprite sprite = sprites[currentPrefabIndex];
        sr.sprite = sprite;
        sr.color = new Color(1, 1, 1, 0.8f);
    }

    private void onRightBtnClick()
    {
        if (currentPrefabIndex + 1 == sprites.Length)
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
        if (currentPrefabIndex == 0)
        {
            currentPrefabIndex = sprites.Length - 1;
        }
        else
        {
            currentPrefabIndex -= 1;
        }
        setThumbnail();
    }
}
