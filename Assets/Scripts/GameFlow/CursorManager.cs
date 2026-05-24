using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance;

    private int uiOpenCount = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        ForceHideCursor();
    }

    public void OpenUI()
    {
        uiOpenCount++;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseUI()
    {
        uiOpenCount--;

        if (uiOpenCount <= 0)
            ForceHideCursor();
    }

    public void ForceHideCursor()
    {
        uiOpenCount = 0;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}