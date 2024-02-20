using System.Collections.Generic;
using System;
using UnityEngine;

namespace SaveLoadSystem
{
  [System.Serializable]
  public class ActivityData
  {
    public List<string> bookmarkedTopics = new List<string>();
    public List<string> bookmarkedLocations = new List<string>();
    public string currentTour; //currently active tour
    public List<OpenExercise> openExercises = new List<OpenExercise>(); //List of all started tours and their open Exercises
    public List<ExerciseOrder> finishedExercises = new List<ExerciseOrder>(); //List of all started tours and their finished Exercises
    public ActiveExercise currentExercise; //currently active exercise
    public List<string> activeTours = new List<string>(); //exercises that are not finished yet
    public List<string> finishedTours = new List<string>(); //exercises that are finished
    public string GetRandomExerciseByTourIdAndRemove(string tourId)
    {
      OpenExercise foundExercise = openExercises.Find(exercise => exercise.tourId == tourId);

      if (foundExercise != null && foundExercise.exerciseIds.Count > 0)
      {
        return foundExercise.GetRandomExerciseAndRemove();
      }
      else
      {
        return "";
      }
    }
    public void StartExercise(Tour currentTourObj){
      if (currentExercise == null || currentTour != currentExercise.tourId){
        //currentExercise is not from currentTour
        Debug.Log("currentExercise is not from currentTour");
        if (openExercises.Exists(e => e.tourId == currentTour)){
          //currentTour has open Exercises
          Debug.Log("currentTour has open Exercises");
          string nextExercise = GetRandomExerciseByTourIdAndRemove(currentTour);
          if (nextExercise != ""){
            //nextExercise available
            Debug.Log($"nextExercise available: {nextExercise}");
            setCurrentExerciseByTour(currentTourObj, nextExercise);
          } else{
            //no nextExercise available
            Debug.Log("This should never happen - Spooky");
          }
        } else {
          //currentTour has no open Exercises
          Debug.Log("currentTour has no open Exercises - lets create them");
          List<string> ids = new List<string>();
          foreach (var ex in currentTourObj.exercises)
          {
            ids.Add(ex.id);
          }
          var openEx = new OpenExercise() { tourId = currentTour, exerciseIds = ids };
          openExercises.Add(openEx);
          string nextExercise = GetRandomExerciseByTourIdAndRemove(currentTour);
          setCurrentExerciseByTour(currentTourObj, nextExercise);
        }
      }
      //no else-statemet needed nothing happens when currentExercise is set
    }

    public void FinishExercise(){
      int index = finishedExercises.FindIndex(e => e.tourId == currentExercise.tourId);
      if (index >= 0)
      {
        // tourId already exists in finishedExercises
        finishedExercises[index].exerciseIds.Add(currentExercise.exercise.id);
      } else {
        List<string> idList = new List<string>();
        idList.Add(currentExercise.exercise.id);
        ExerciseOrder done = new ExerciseOrder(){ tourId = currentTour, exerciseIds = idList };
        finishedExercises.Add(done);
      }
      currentExercise = null;
    }

    private void setCurrentExerciseByTour(Tour tour, string exerciseId)
    {
      Debug.Log("YOLO setCurrentExerciseByTour");
      var exercise = tour.exercises.Find(e => e.id == exerciseId);
      currentExercise = new ActiveExercise() { tourId = tour.id, exercise = exercise };
    }

    public int getProgress(Tour tour){
      if(openExercises.Exists(e => e.tourId == tour.id)){
        int multiplier = 100 / tour.exercises.Count;
        int diff = tour.exercises.Count - openExercises.Find(e => e.tourId == tour.id).exerciseIds.Count;
        return diff * multiplier;
      } else if (finishedTours.Contains(tour.id))
      {
        return 100;
      } else
      {
        return 0;
      }
    }

    public void StartTour(string tourId){
      currentTour = tourId;
      if (!activeTours.Contains(tourId))
      {
        activeTours.Add(tourId);
      }
    }

    public void FinishTour()
    {
      int indexFinished = finishedExercises.FindIndex(e => e.tourId == currentTour);
      finishedExercises.RemoveAt(indexFinished);
      int indexOpen = openExercises.FindIndex(e => e.tourId == currentTour);
      openExercises.RemoveAt(indexOpen);
      activeTours.Remove(currentTour);
      if(!finishedTours.Contains(currentTour)){
        finishedTours.Add(currentTour);
      }
      //currentExercise = null;
      currentTour = "";
    }
  }

  [System.Serializable]
  public class ActiveExercise{
    public string tourId;
    public Exercise exercise; 
  }

  [System.Serializable]
  public class ExerciseOrder{
    public string tourId;
    public List<string> exerciseIds;
  }

  [System.Serializable]
  public class OpenExercise : ExerciseOrder{
    public string GetRandomExerciseAndRemove()
    {
      if (exerciseIds == null || exerciseIds.Count == 0)
      {
        return "";
      }

      System.Random rnd = new System.Random();
      int index = rnd.Next(exerciseIds.Count); // Zufälligen Index generieren
      string selectedExerciseId = exerciseIds[index]; // Element an diesem Index speichern
      exerciseIds.RemoveAt(index); // Element aus der Liste entfernen

      return selectedExerciseId;
    }
  }
}
