using UnityEngine;

public class ChestUIController : MonoBehaviour, IInteractable
{
    public GameObject chestPanel;

    private bool isOpen = false;
    private Chest chestData;
    private ChestUI uiScript;

    private void Awake()
    {
        chestData = GetComponent<Chest>();
        uiScript = chestPanel.GetComponent<ChestUI>();
        if (chestPanel != null) chestPanel.SetActive(false);
    }

    public void Interact()
    {
        isOpen = !isOpen;
        chestPanel.SetActive(isOpen);

        if (isOpen)
        {
            chestData.GenerateInventory();
            uiScript.RefreshUI(chestData.currentInventory);

            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            if (CursorManager.Instance != null) CursorManager.Instance.OpenUI();
        }
        else
        {
            Time.timeScale = 1f;
            if (CursorManager.Instance != null) CursorManager.Instance.CloseUI();
        }
    }
}