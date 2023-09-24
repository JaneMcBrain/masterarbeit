using System.Collections.Generic;

namespace SaveLoadSystem{
  [System.Serializable]
  public class ActivityData
  {
    public string currentTour; //currently active tour
    public ActiveExercise currentExercise; //currently active exercise
    public List<ActiveExercise> activeExercises = new List<ActiveExercise>(); //exercises that are not finished yet
  }

  [System.Serializable]
  public class ActiveExercise{
    public string tourId;
    public Exercise exercise;
  }
}
