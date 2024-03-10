using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UIElements;
using SaveLoadSystem;


[System.Serializable]
public class AllFacesData
{
    public List<FaceImage> images;
}

[System.Serializable]
public class FaceImage
{
    public string imagePath;

    public Vector2 size;
    public FaceData faceData;
}

[System.Serializable]
public class FaceData
{
    public List<RectangleData> faces;
    public List<LandmarkData> landmarks;
}

[System.Serializable]
public class LandmarkData
{
    public List<float> x;
    public List<float> y;
}

[System.Serializable]
public class RectangleData
{
    public float x;
    public float y;
    public float width;
    public float height;
}


public class FaceFilterScript : MonoBehaviour
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
        foreach (FaceImage faceImage in allFacesData.images)
        {
            string imagePath = faceImage.imagePath;
            Vector2 imageSize = faceImage.size;
            if (imagePath == searchedImage){
                List<LandmarkData> landmarks = faceImage.faceData.landmarks;
                multiplier = new Vector2(trackedImage.size.x / imageSize.x, trackedImage.size.y / imageSize.y);
                Transform imgTransform = trackedImage.transform;
                List<RectangleData> faces = faceImage.faceData.faces;
                for (int i = 0; i < faces.Count; i++)
                {
                    string key = imagePath + "_face_" + i;
                    if (!instantiatedRects.ContainsKey(key)){
                        GameObject rectangle;
                        rectangle = Instantiate(WhiteRect, imgTransform);
                        ApplyFaceFilter(rectangle, landmarks[i], trackedImage, faces[i]);
                        instantiatedRects[key] = rectangle;
                        instantiatedRects[key].SetActive(true);
                    } else {
                        ApplyFaceFilter(instantiatedRects[key], landmarks[i], trackedImage, faces[i]);
                    }
                }
            }
        }
    }

    void setRectPosition(ARTrackedImage img, RectangleData face, GameObject gameO)
    {
        Vector3 posZero = img.transform.position - new Vector3(img.size.x / 2, -img.size.y / 2, 0);
        Vector3 faceCoordinates = new Vector3(face.x * multiplier.x + (face.width / 2), -(face.y * multiplier.y) - (face.height / 2), 0);
        gameO.transform.position = posZero + faceCoordinates;
    }

    void ApplyFaceFilter(GameObject rectangle, LandmarkData landmarksData, ARTrackedImage img, RectangleData rec)
    {
        Vector2 recTopLeft = new Vector2(rec.x, rec.y);
        Vector2 recBottomLeft = new Vector2(landmarksData.x[29], rec.y);
        float zPos = img.transform.position.z;

        // Rechteckgröße neu berechnen
        float width = multiplier.x * rec.width;
        float height = multiplier.y * Vector2.Distance(recTopLeft, recBottomLeft);
        rectangle.transform.localScale = new Vector3(width, height, zPos);
        RectangleData face = new RectangleData
        {
            x = rec.x,
            y = rec.y,
            width = width,
            height = height
        };
        // Rechteckposition neu berechnen
        setRectPosition(img, face, rectangle);
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
