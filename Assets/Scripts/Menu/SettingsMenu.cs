using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("واجهات القوائم")]
    public GameObject settingsPanel;

    [Header("إعدادات الصوت")]
    public AudioMixer mainMixer;

    // متغيرات لقراءة قيمة الصوت الحالية
    private float currentMusicVol;
    private float currentSFXVol;

    // --- دوال الشاشة والصوت ---
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

    // --- دالة اللغة ---
    public void SetLanguage(int languageIndex)
    {
        if (languageIndex == 0)
        {
            Debug.Log("تم تغيير اللغة إلى: English");
        }
        else if (languageIndex == 1)
        {
            Debug.Log("تم تغيير اللغة إلى: العربية");
        }
    }

    // --- دوال الإغلاق والحفظ ---
    public void SaveSettings()
    {
        // 1. قراءة القيم الحالية من المكسر
        mainMixer.GetFloat("MusicVolume", out currentMusicVol);
        mainMixer.GetFloat("SFXVolume", out currentSFXVol);

        // 2. حفظ القيم في ذاكرة اللعبة (PlayerPrefs)
        PlayerPrefs.SetFloat("SavedMusic", currentMusicVol);
        PlayerPrefs.SetFloat("SavedSFX", currentSFXVol);
        PlayerPrefs.Save();

        Debug.Log("تم حفظ الإعدادات بنجاح!");

        // 3. إغلاق اللوحة
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