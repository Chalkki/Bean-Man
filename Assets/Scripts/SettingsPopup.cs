using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SettingsPopup : MonoBehaviour
{
    [SerializeField] GameObject SettingsPanel;

    private void Start()
    {
        SettingsPanel.SetActive(false);
    }

    public void Open()
    {
        // open the settings
        Time.timeScale = 0f;
        SettingsPanel.SetActive(true);
    }
    public void Close()
    {
        // close the popup window
        Time.timeScale = 1f;
        SettingsPanel.SetActive(false);
    }

    public void ReturnMainMenu()
    {
        // return to main menu here
    }

}
