using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("واجهات القوائم")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject instructionsPanel;
    public GameObject creditsPanel;

    [Header("أصوات الواجهة")]
    public AudioSource uiAudioSource; // مصدر الصوت
    public AudioClip hoverSound;      // ملف صوت التمرير (الهوفر)
    public AudioClip clickSound;      // ملف صوت الضغطة

    // --- دوال تشغيل الأصوات الجديدة ---
    public void PlayHoverSound()
    {
        if (uiAudioSource != null && hoverSound != null)
            uiAudioSource.PlayOneShot(hoverSound);
    }

    public void PlayClickSound()
    {
        if (uiAudioSource != null && clickSound != null)
            uiAudioSource.PlayOneShot(clickSound);
    }

    // --- باقي دوالك القديمة (Play, OpenSettings, الخ...) ---
    public void StartGame()
    {
        SceneManager.LoadScene("Village");
    }

    public void OpenSettings() { if (settingsPanel != null) settingsPanel.SetActive(true); }
    public void OpenInstructions() { if (instructionsPanel != null) instructionsPanel.SetActive(true); }
    public void OpenCredits() { if (creditsPanel != null) creditsPanel.SetActive(true); }

    public void BackToMainMenu()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ExitGame()
    {
        Debug.Log("اللاعب خرج من اللعبة!");
        Application.Quit();
    }
}