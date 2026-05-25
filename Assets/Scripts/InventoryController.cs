using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    public InputActionReference inventoryAction;

    [SerializeField] private InventoryPage inventoryUI;
    public int inventorySize = 20;

    private bool isOpen;

    private void Awake()
    {
        FindInventory();

        if (inventoryUI != null)
        {
            inventoryUI.InitializeInventoryUI(inventorySize);
            inventoryUI.Hide();
        }
    }

    private void Update()
    {
        if (inventoryAction == null || inventoryAction.action == null)
            return;

        if (!inventoryAction.action.WasPressedThisFrame())
            return;

        FindInventory();

        if (inventoryUI == null)
        {
            Debug.LogError("InventoryController: InventoryPage not found.");
            return;
        }

        isOpen = !isOpen;

        if (isOpen)
        {
            inventoryUI.Show();
            Time.timeScale = 0f;

            if (CursorManager.Instance != null)
                CursorManager.Instance.OpenUI();
        }
        else
        {
            inventoryUI.Hide();
            Time.timeScale = 1f;

            if (CursorManager.Instance != null)
                CursorManager.Instance.CloseUI();
        }
    }

    private void FindInventory()
    {
        if (inventoryUI == null)
            inventoryUI = FindFirstObjectByType<InventoryPage>(FindObjectsInactive.Include);
    }
}