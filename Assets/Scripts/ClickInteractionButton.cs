using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickInteractionButton : MonoBehaviour
{
    private void SaveLevel(int level)
    {
        string key = "level";
        PlayerPrefs.SetInt(key, level);
    }

    public string sceneName;
    public int level;
    // Update is called once per frame
    public void OpenScene()
    {
        SceneManager.LoadScene(sceneName);
        SaveLevel(level);
    }
}
