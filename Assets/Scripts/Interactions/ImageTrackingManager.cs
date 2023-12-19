using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using SaveLoadSystem;
using System;


public class ImageTrackingManager : MonoBehaviour
{
  private ARTrackedImageManager imageManager;
  public XRReferenceImageLibrary xrReferenceLibrary;
  
  public event Action<ARTrackedImage> OnImageUpdated;

  private void Start()
  {
    InitializeImageTracking();
  }

  private void InitializeImageTracking()
  {
    imageManager = GetComponent<ARTrackedImageManager>();
    imageManager.referenceLibrary = imageManager.CreateRuntimeLibrary(xrReferenceLibrary);
    imageManager.enabled = true;
    imageManager.trackedImagesChanged += OnTrackedImagesChanged;
  }

  private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
  {
    SaveGameManager.LoadState();
    var searchedImage = SaveGameManager.CurrentActivityData.currentExercise.exercise.image;
    foreach (var trackedImage in eventArgs.updated)
    {
      var artworkName = trackedImage.referenceImage.name;
      if (trackedImage.trackingState == TrackingState.Tracking && artworkName == searchedImage)
      {
        OnImageUpdated?.Invoke(trackedImage);
      }
    }
  }
}
