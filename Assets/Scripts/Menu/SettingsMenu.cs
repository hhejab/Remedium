using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;

    [Header("Audio Mixer")]
    public AudioMixer mainMixer;

    
    private float currentMusicVol;
    private float currentSFXVol;

   
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetMusicVolume(float volume)
    {
        mainMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume(float volume)
    {
        mainMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }


    public void SetLanguage(int languageIndex)
    {
        if (languageIndex == 0)
        {
           
        }
        else if (languageIndex == 1)
        {
            
        }
    }


    public void SaveSettings()
    {
    
        mainMixer.GetFloat("MusicVolume", out currentMusicVol);
        mainMixer.GetFloat("SFXVolume", out currentSFXVol);

        PlayerPrefs.SetFloat("SavedMusic", currentMusicVol);
        PlayerPrefs.SetFloat("SavedSFX", currentSFXVol);
        PlayerPrefs.Save();

  

        CloseMenu();
    }

    public void DeclineSettings()
    {
        CloseMenu();
    }

    private void CloseMenu()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }
}