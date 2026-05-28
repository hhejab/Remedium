using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel; 

    public static bool isPaused = false;

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuPanel.SetActive(false);
       
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

   
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void BackToMainMenu()
    {
       
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

    
        Time.timeScale = 1f;
        isPaused = false;

       
        SceneManager.LoadScene("MainMenu_Final"); 
    }
}