using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference inventoryAction;

    [Header("Inventory Settings")]
    [SerializeField] private InventoryPage inventoryUI;
    public int inventorySize = 20;

    private bool isOpen;

    private void Awake()
    {
        // Hide on start
        if (inventoryUI != null)
        {
            inventoryUI.Hide();
            inventoryUI.InitializeInventoryUI(inventorySize);
        }
    }

    private void OnEnable()
    {
        if (inventoryAction != null)
            inventoryAction.action.Enable();
    }

    private void OnDisable()
    {
        if (inventoryAction != null)
            inventoryAction.action.Disable();
    }

    private void Update()
    {
        // Using the same logic as your SkillBook
        if (inventoryAction != null && inventoryAction.action.WasPressedThisFrame())
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (isOpen)
            CloseInventory();
        else
            OpenInventory();
    }

    public void OpenInventory()
    {
        isOpen = true;
        inventoryUI.Show();
        
        // Match the SkillBook feel: Pause time and show mouse
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void CloseInventory()
    {
        isOpen = false;
        inventoryUI.Hide();

        // Resume game and hide mouse
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}