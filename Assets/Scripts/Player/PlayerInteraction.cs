using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionReference interactAction;
    private IInteractable currentInteractable;

    private void OnEnable() => interactAction?.action.Enable();
    private void OnDisable() => interactAction?.action.Disable();

    private void Update()
    {
        // WasPressedThisFrame guarantees it only fires exactly ONCE per button press
        if (interactAction != null && interactAction.action.WasPressedThisFrame())
        {
            if (currentInteractable != null)
            {
                currentInteractable.Interact();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable) && interactable == currentInteractable)
        {
            currentInteractable = null;
        }
    }
}