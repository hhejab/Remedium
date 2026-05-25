using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    [Header("واجهات التوقف")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel; // الخانة الجديدة للوحة الإعدادات

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
        // تأمين إضافي: إذا قفلنا التوقف، نقفل الإعدادات معها عشان ما تعلق بالشاشة
        if (settingsPanel != null) settingsPanel.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
    }

    // --- الدالة الجديدة لزر الإعدادات ---
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void BackToMainMenu()
    {
        // 1. نقفل قائمة التوقف أولاً عشان ما تسافر معنا!
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }

        // 2. نرجع الزمن لطبيعته
        Time.timeScale = 1f;
        isPaused = false;

        // 3. ننتقل للمين منيو
        SceneManager.LoadScene("MainMenu_Final"); // تأكدي إن هذا اسم مشهد البداية عندكم
    }
}