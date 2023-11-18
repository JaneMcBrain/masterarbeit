using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UIElements;
using UnityEngine.XR.ARKit;

public class ChangeObjectScriptFace : MonoBehaviour
{
    // Start is called before the first frame update
    private ARFaceManager faceManager;
    public List<Material> faceMaterials = new List<Material>();

    public GameObject UI;
    private int faceMaterialIndex;
    private VisualElement uiDocument;

    private int numberOfDetectedFaces;

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

        var arCamera = FindObjectOfType<ARCameraManager>();
        faceManager = GetComponent<ARFaceManager>();
        faceManager.facesChanged += OnFacesChanged;
        var ft = GetComponent<ARKitFaceSubsystem>();
        var track = GetComponent<TrackingMode>();
        Debug.Log("Tracking Mode: " + track);

        if (arCamera != null)
        {
            // Protokollieren Sie die Kameraeinstellungen.
            Debug.Log("Camera Settings:");
            Debug.Log("Camera requestedFacingDirection: " + arCamera.requestedFacingDirection);
            Debug.Log("Camera currentFacingDirection: " + arCamera.currentFacingDirection);
            Debug.Log("Camera requestedLightEstimation: " + arCamera.requestedLightEstimation);
        }
        else
        {
            Debug.LogWarning("ARCameraManager not found.");
        }

        Debug.Log("Face Manager: " + faceManager.enabled);

        if (faceManager != null)
        {
            foreach (ARFace face in faceManager.trackables)
            {
                Debug.Log("Face trackableId: " + face.trackableId);
            }
        }
        else
        {
            Debug.LogError("ARFaceManager not found on this GameObject.");
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
        //every face that is tracked needs an update
        foreach (ARFace face in faceManager.trackables)
        {
            Debug.Log("Face tracked and changed to: " + faceMaterialIndex);
            face.GetComponent<Renderer>().material = faceMaterials[faceMaterialIndex];
        }

    }

    void OnFacesChanged(ARFacesChangedEventArgs eventArgs)
    {
        numberOfDetectedFaces = eventArgs.added.Count;

        if (numberOfDetectedFaces > 0)
        {
            Debug.Log("Gesicht(er) erkannt: " + numberOfDetectedFaces);
            // Hier könntest du zusätzliche Aktionen durchführen, z.B. den Filter aktivieren
        }
        else
        {
            Debug.Log("Kein Gesicht erkannt");
            // Hier könntest du zusätzliche Aktionen durchführen, z.B. den Filter deaktivieren
        }
    }
}
