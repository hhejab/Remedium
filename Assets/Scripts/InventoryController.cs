using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryController : MonoBehaviour
{
    public InputActionReference inventoryAction;
    [SerializeField] private InventoryPage inventoryUI;
    public int inventorySize = 20;
    private bool isOpen;

    private void Awake() {
        if (inventoryUI != null) inventoryUI.InitializeInventoryUI(inventorySize);
    }

    private void Update() {
        if (inventoryAction.action.WasPressedThisFrame()) {
            isOpen = !isOpen;
            if (isOpen) { inventoryUI.Show(); Time.timeScale = 0; Cursor.visible = true; Cursor.lockState = CursorLockMode.None; }
            else { inventoryUI.Hide(); Time.timeScale = 1; Cursor.visible = false; Cursor.lockState = CursorLockMode.Locked; }
        }
    }
}