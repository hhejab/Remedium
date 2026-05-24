using UnityEngine;

public class ChestUIController : MonoBehaviour, IInteractable
{
    [Header("UI Reference")]
    public GameObject chestPanel;

    private bool isOpen = false;

    private void Start()
    {
        if (chestPanel != null)
            chestPanel.SetActive(false);
    }

    public void Interact()
    {
        ToggleChest();
    }

    public void ToggleChest()
    {
        if (chestPanel == null)
        {
            Debug.LogError("ChestUIController: Chest Panel is missing.");
            return;
        }

        isOpen = !isOpen;
        chestPanel.SetActive(isOpen);

        if (isOpen)
        {
            Time.timeScale = 0f;

            if (CursorManager.Instance != null)
                CursorManager.Instance.OpenUI();
        }
        else
        {
            Time.timeScale = 1f;

            if (CursorManager.Instance != null)
                CursorManager.Instance.CloseUI();
        }
    }

    private void OnDisable()
    {
        if (isOpen)
        {
            isOpen = false;
            Time.timeScale = 1f;

            if (CursorManager.Instance != null)
                CursorManager.Instance.CloseUI();
        }
    }
}