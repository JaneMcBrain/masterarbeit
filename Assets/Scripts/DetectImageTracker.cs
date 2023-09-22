using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.SceneManagement;
using SaveLoadSystem;

public class DetectImageTracker : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager _trackedImageManager;

    [SerializeField]
    public GameObject[] ArObjects;
    public GameObject CorrectImage;
    public GameObject WrongImage;
    public GameObject _prefabToSpawn;

    private readonly Dictionary<string, GameObject> _instantiatedArtworks = new Dictionary<string, GameObject>();
    private readonly Dictionary<string, GameObject> _instantiatedFeedback = new Dictionary<string, GameObject>();

    void Awake() => _trackedImageManager = GetComponent<ARTrackedImageManager>();

    private void OnEnable() => _trackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;

    private void OnDisable() => _trackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs){
        var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.image;
        //this is to add the artwork on top
        foreach (ARTrackedImage trackedImage in eventArgs.added){
            var artworkName = trackedImage.referenceImage.name;
            foreach (var currentObject in ArObjects)
            {
                if(string.Compare(currentObject.name, artworkName, StringComparison.OrdinalIgnoreCase) == 0 
                    && !_instantiatedArtworks.ContainsKey(artworkName)){
                    var newArObj = Instantiate(currentObject, trackedImage.transform);
                    _instantiatedArtworks[artworkName] = newArObj;
                }
            }
        }

        foreach(var trackedImage in eventArgs.updated){
            var trackImageName = trackedImage.referenceImage.name;
            //check if trackedImages is tracked && is the correct artwork
            if (trackedImage.trackingState == TrackingState.Tracking && trackImageName == searchedImage)
            {
                //if artwork is correct
                var newArObj = Instantiate(CorrectImage, trackedImage.transform);
                var keyRight = "right_" + trackImageName;
                if (!_instantiatedFeedback.ContainsKey(keyRight)){
                    _instantiatedFeedback[keyRight] = newArObj;
                    _instantiatedFeedback[keyRight].SetActive(true);
                }
            }
            if (trackedImage.trackingState == TrackingState.Tracking && trackImageName != searchedImage)
            {
                var newArObj = Instantiate(WrongImage, trackedImage.transform);
                var keyWrong = "wrong_" + trackImageName;
                if (!_instantiatedFeedback.ContainsKey(keyWrong))
                {
                    _instantiatedFeedback[keyWrong] = newArObj;
                    _instantiatedFeedback[keyWrong].SetActive(true);
                }
            }
            //Adds Artwork as well
            //_instantiatedArtworks[trackedImage.referenceImage.name].SetActive(trackedImage.trackingState == TrackingState.Tracking);
        }
        //reset everything on remove
        foreach (var trackedImage in eventArgs.removed)
        {
            Destroy(_instantiatedArtworks[trackedImage.referenceImage.name]);
            Destroy(_instantiatedFeedback["right_" + trackedImage.referenceImage.name]);
            _instantiatedArtworks.Remove(trackedImage.referenceImage.name);
            _instantiatedFeedback.Remove("right_" + trackedImage.referenceImage.name);
        }
    }

    private void SpawnPrefab(ARTrackedImage trackedImage)
    {
        // Check if the image is the one we want to track
        if (trackedImage.referenceImage.name == "floetenkonzert")
        {
            // Spawn the prefab at the same position and rotation as the tracked image
            Instantiate(_prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation);

            //SceneManager.LoadScene("ChangeStyleScene");
        }

        if (trackedImage.referenceImage.name == "voegel")
        {
            // Spawn the prefab at the same position and rotation as the tracked image
            Instantiate(_prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation);
            //SceneManager.LoadScene("CuttingScene");
        }

        if (trackedImage.referenceImage.name == "parade")
        {
            // Spawn the prefab at the same position and rotation as the tracked image
            Instantiate(_prefabToSpawn, trackedImage.transform.position, trackedImage.transform.rotation);
            //SceneManager.LoadScene("ObjectChangeScene");
        }
    }
}
