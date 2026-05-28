using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject settingsPanel;
    public GameObject instructionsPanel;
    public GameObject creditsPanel;

    [Header("Audio")]
    public AudioSource uiAudioSource; 
    public AudioClip hoverSound;      
    public AudioClip clickSound;     

   
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


 public void StartGame()
{
    Time.timeScale = 1f;
    SceneSpawnManager.nextSpawnPointName = "VillageSpawn";
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
        
        Application.Quit();
    }
}