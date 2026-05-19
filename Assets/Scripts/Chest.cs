using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject chestUIPanel; // Drag your Chest UI Panel here in the Inspector
    private bool isOpen = false;

    public void Interact()
    {
        isOpen = !isOpen;
        
        if (chestUIPanel != null)
        {
            chestUIPanel.SetActive(isOpen);
            
            // Pause game and show mouse (matches your SkillBook logic)
            Time.timeScale = isOpen ? 0f : 1f;
            Cursor.visible = isOpen;
            Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        }
        
        Debug.Log(isOpen ? "Chest Opened" : "Chest Closed");
    }
}