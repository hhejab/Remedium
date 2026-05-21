using UnityEngine;

public class ChestUIController : MonoBehaviour, IInteractable
{
    [Header("UI Reference")]
    public GameObject chestPanel;
    
    private bool isOpen = false;

    private void Start()
    {
        // Start with the chest closed
        if (chestPanel != null) chestPanel.SetActive(false);
    }

    public void Interact()
    {
        ToggleChest();
    }

    public void ToggleChest()
    {
        isOpen = !isOpen;
        chestPanel.SetActive(isOpen);

        // This pauses the game and shows the mouse, just like your SkillBook
        if (isOpen)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}