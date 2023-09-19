using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class ScanOverlayScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var overlayUI = gameObject.GetComponent<UIDocument>().rootVisualElement;
        //overlayUI.Q<VisualElement>("ScanFooter").clicked += () => ToggleInfo(overlayUI);
        overlayUI.Q<VisualElement>("ScanFooter").AddManipulator(new Clickable(evt => ToggleInfo(overlayUI)));
        overlayUI.Q<Button>("BackButton").clicked += () => SceneManager.LoadScene("InteractionNavi");
    }

    void ToggleInfo(VisualElement root){
        var infoText = root.Q<Label>(className: "hidden");
        if (infoText != null)
        {
            infoText.RemoveFromClassList("hidden");
        } else {
            root.Q<Label>("FooterInfoText").AddToClassList("hidden");
        }
    }
}
