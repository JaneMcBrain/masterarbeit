using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UIElements;
using SaveLoadSystem;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARSubsystems;

public class ChangeObjectScript : MonoBehaviour
{
    // Start is called before the first frame update
    private ARFaceManager faceManager;
    private ARKitCameraSubsystem camera;
    public List<Material> faceMaterials = new List<Material>();
    private int faceMaterialIndex;
    private VisualElement uiDocument;
    void Start()
    {

        var arCamera = FindObjectOfType<ARCameraManager>();
        faceManager = GetComponent<ARFaceManager>();
        camera = GetComponent<ARKitCameraSubsystem>();
        var cam = GetComponent<ARCameraManager>();
        var ft = GetComponent<ARKitFaceSubsystem>();
        var session = GetComponent<ARKitSessionSubsystem>();
        var track = GetComponent<TrackingMode>();
        var subsystem = GetComponent<ARKitSessionSubsystem>();
        Debug.Log("Tracking Mode: " + track);
        Debug.Log("Subsystem currentTrackingMode: " + subsystem.currentTrackingMode);
        Debug.Log("Subsystem requestedTrackingMode: " + subsystem.requestedTrackingMode);
        Debug.Log("Subsystem requestedTrackingMode: " + subsystem.trackingState);

        if (arCamera != null)
        {
            // Protokollieren Sie die Kameraeinstellungen.
            Debug.Log("Camera Settings:");
            Debug.Log("Camera Face Direction: " + arCamera.requestedFacingDirection);
            Debug.Log("Camera Light Estimation: " + arCamera.requestedLightEstimation);
            Debug.Log("Camera Light Estimation: " + arCamera);
        }
        else
        {
            Debug.LogWarning("ARCameraManager not found.");
        }

        Debug.Log("ARCameraManager");
        Debug.Log(cam.currentConfiguration);
        Debug.Log(cam.requestedFacingDirection);
        Debug.Log("ARKitCameraSubsystem");
        Debug.Log(camera.currentConfiguration);

        Debug.Log("HALLOOOOOOOOOOOO");
        Debug.Log(ft.subsystemDescriptor);
        Debug.Log(session.currentConfiguration);

        uiDocument = GetComponent<UIDocument>().rootVisualElement;
        uiDocument.Q<VisualElement>("ChangeObjectButtons").RemoveFromClassList("hidden");
        var changeBtn1 = uiDocument.Q<Button>("ChangeObject1");
        var changeBtn2 = uiDocument.Q<Button>("ChangeObject2");
        var changeBtn3 = uiDocument.Q<Button>("ChangeObject3");
        changeBtn1.clicked += () => onChangeObject(changeBtn1, 0);
        changeBtn2.clicked += () => onChangeObject(changeBtn2, 1);
        changeBtn3.clicked += () => onChangeObject(changeBtn3, 2);
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
}
