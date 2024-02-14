using System.Collections.Generic;
using System;

namespace SaveLoadSystem
{
  [System.Serializable]
  public class ActivityData
  {
    public string currentTour; //currently active tour
    public List<OpenExercise> openExercises = new List<OpenExercise>();
    public List<OpenExercise> startedExercises = new List<OpenExercise>();

    public List<string> bookmarkedTopics = new List<string>();
    public List<string> bookmarkedLocations = new List<string>();
    public ActiveExercise currentExercise; //currently active exercise
    public List<ActiveExercise> activeExercises = new List<ActiveExercise>(); //exercises that are not finished yet
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
  }

  [System.Serializable]
  public class ActiveExercise{
    public string tourId;
    public Exercise exercise; 
  }

  [System.Serializable]
  public class OpenExercise{
    public string tourId;
    public List<string> exerciseIds;

    public string GetRandomExerciseAndRemove()
    {
      if (exerciseIds == null || exerciseIds.Count == 0)
      {
        throw new InvalidOperationException("Es gibt keine Übungen zur Auswahl.");
      }

      Random rnd = new Random();
      int index = rnd.Next(exerciseIds.Count); // Zufälligen Index generieren
      string selectedExerciseId = exerciseIds[index]; // Element an diesem Index speichern
      exerciseIds.RemoveAt(index); // Element aus der Liste entfernen

      return selectedExerciseId;
    }
  }
}
