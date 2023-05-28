using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HelpButton : MonoBehaviour
{
    public GameObject HelpPanel;

    public void ToggleHelpPanel()
    {
        if(HelpPanel != null){
            bool isActive = HelpPanel.activeSelf;
            HelpPanel.SetActive(!isActive);
        }
    }
}
