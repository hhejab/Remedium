using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionReference interactAction; // Assign the 'E' action here
    private IInteractable currentInteractable;

    private void OnEnable() => interactAction.action.Enable();
    private void OnDisable() => interactAction.action.Disable();

    private void Update()
    {
        // If 'E' is pressed and we are near something interactable
        if (interactAction.action.WasPressedThisFrame() && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
            Debug.Log("Press E to interact with " + other.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            if (currentInteractable == interactable)
                currentInteractable = null;
        }
    }
}