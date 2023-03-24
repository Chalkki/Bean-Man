using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
public class SettingsPopup : MonoBehaviour
{
    [SerializeField] GameObject SettingsPanel;
    [SerializeField] Slider volumnBar;
    private void Start()
    {
        volumnBar.value = PlayerPrefs.GetFloat("Volumn", 1f);
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
        Controller.score = 0;
        SceneManager.LoadScene("MainMenu");
    }

}
