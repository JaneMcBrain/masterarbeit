using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UIElements;
using SaveLoadSystem;


[System.Serializable]
public class AllFacesData
{
    public List<FaceImage> allFacesData;
}

[System.Serializable]
public class FaceImage
{
    public string imagePath;
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
    // Start is called before the first frame update
    private ARFaceManager faceManager;
    public List<Material> faceMaterials = new List<Material>();

    public GameObject UI;
    private int faceMaterialIndex;
    private VisualElement uiDocument;

    private int numberOfDetectedFaces;
    public TextAsset allFacesDataJson;

    void OnEnable()
    {
        uiDocument = UI.GetComponent<UIDocument>().rootVisualElement;
        uiDocument.Q<VisualElement>("ChangeObjectButtons").RemoveFromClassList("hidden");
        var changeBtn1 = uiDocument.Q<Button>("ChangeObject1");
        var changeBtn2 = uiDocument.Q<Button>("ChangeObject2");
        var changeBtn3 = uiDocument.Q<Button>("ChangeObject3");
        changeBtn1.clicked += () => onChangeObject(changeBtn1, 0);
        changeBtn2.clicked += () => onChangeObject(changeBtn2, 1);
        changeBtn3.clicked += () => onChangeObject(changeBtn3, 2);

    }

    void highlightFaces(){
        if (allFacesDataJson != null)
        {
            // Lade JSON-Datei und deserialisiere sie
            AllFacesData allFacesData = JsonUtility.FromJson<AllFacesData>(allFacesDataJson.text);

            // Iteriere über alle Gesichtsdaten
            foreach (FaceImage faceImage in allFacesData.allFacesData)
            {
                string imagePath = faceImage.imagePath;
                SaveGameManager.LoadState();
                var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;

                if (imagePath == searchedImage){
                    // Zugriff auf Gesichtsdaten und Landmarks
                    FaceData faceData = faceImage.faceData;
                    // Hier muss über jedes Face iteriert werden und ein Rectangle raufgesetzt werden
                }
            }
        }
        else
        {
            Debug.LogError("All Faces Data JSON not assigned!");
        }
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
