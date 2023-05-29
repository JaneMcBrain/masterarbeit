using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;

public class DetectImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager _trackedImageManager;

    [SerializeField]
    public GameObject _prefabToSpawn;

    void Awake() => _trackedImageManager = GetComponent<ARTrackedImageManager>();

    private void OnEnable() => _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

    private void OnDisable() => _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            if (trackedImage.trackingState == TrackingState.Tracking)
            {
                SpawnPrefab(trackedImage);
            }
        }
    }

    private void SpawnPrefab(ARTrackedImage trackedImage)
    {
        // Check if the image is the one we want to track
        if (trackedImage.referenceImage.name == "floetenkonzert")
        {
            // Spawn the prefab at the same position and rotation as the tracked image
            Instantiate(_prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation);
            System.Threading.Thread.Sleep(3000);
            SceneManager.LoadScene("ChangeStyleScene");
        }
    }
}
