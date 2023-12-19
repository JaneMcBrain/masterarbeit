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


public class ChangeObjectScriptFace : MonoBehaviour
{

    public List<Material> faceMaterials = new List<Material>();

    public GameObject UI;
    private int faceMaterialIndex;
    private VisualElement uiDocument;

    private int numberOfDetectedFaces;
    public TextAsset jsonFile;

    private ImageTrackingManager imageTrackingManager;
    private Camera cam;
    public GameObject WhiteRect;
    private readonly Dictionary<string, GameObject> instantiatedRects = new Dictionary<string, GameObject>();

    void Start()
    {
        cam = Camera.main;
    }

    void OnEnable()
    {
        imageTrackingManager = gameObject.AddComponent<ImageTrackingManager>();
        imageTrackingManager.OnImageUpdated += OnImageUpdated;

        uiDocument = UI.GetComponent<UIDocument>().rootVisualElement;
        uiDocument.Q<VisualElement>("ChangeObjectButtons").RemoveFromClassList("hidden");
        var changeBtn1 = uiDocument.Q<Button>("ChangeObject1");
        var changeBtn2 = uiDocument.Q<Button>("ChangeObject2");
        var changeBtn3 = uiDocument.Q<Button>("ChangeObject3");
        changeBtn1.clicked += () => onChangeObject(changeBtn1, 0);
        changeBtn2.clicked += () => onChangeObject(changeBtn2, 1);
        changeBtn3.clicked += () => onChangeObject(changeBtn3, 2);

    }

    void OnImageUpdated(ARTrackedImage trackedImage)
    {
        highlightFaces(trackedImage);
    }

    void highlightFaces(ARTrackedImage trackedImage)
    {
        if (jsonFile != null)
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
                if (imagePath == searchedImage){
                    List<RectangleData> faces = faceImage.faceData.faces;
                    for (int i = 0; i < faces.Count; i++)
                    {
                        RectangleData face = faces[i];
                        string key = imagePath + "_face_" + i;
                        Vector2 multiplier = new Vector2(trackedImage.size.x / imageSize.x, trackedImage.size.y / imageSize.y);
                        Transform imgTransform = trackedImage.transform;
                        if (!instantiatedRects.ContainsKey(key)){
                            //Berechnung der Weltgröße je Pixel
                            GameObject rectangle;
                            rectangle = Instantiate(WhiteRect, imgTransform);
                            // Rechteckgröße neu berechnen
                            float recWidth = multiplier.x * face.width;
                            float recHeight = multiplier.y * face.height;
                            rectangle.transform.localScale = new Vector3(recWidth, recHeight, 1f);
                            // Rechteckposition neu berechnen
                            // 1. Rechteck am Bild unten links positionen
                            setObjectPosition(trackedImage, multiplier, face, rectangle);
                            instantiatedRects[key] = rectangle;
                        } else {
                            setObjectPosition(trackedImage, multiplier, face, instantiatedRects[key]);
                        }
                    }
                }
            }
        }
        else
        {
            Debug.LogError("All Faces Data JSON not assigned!");
        }
    }

    void setObjectPosition(ARTrackedImage img, Vector2 multiplier, RectangleData face, GameObject gameO)
    {
        Vector3 pos = img.transform.position - new Vector3(img.size.x / 2, img.size.y / 2, 0);
        Vector3 faceCoordinates = new Vector3(face.x * multiplier.x, face.y * multiplier.y, 0);
        gameO.transform.position = pos + faceCoordinates;
    }

    void onChangeObject(VisualElement btn, int index)
    {
        faceMaterialIndex = index;
        activateButton(btn);
        SwitchFace();
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

    void SwitchFace()
    {
        Debug.Log("SwitchFace");

    }
}
