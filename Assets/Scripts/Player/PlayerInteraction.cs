using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionReference interactAction;

    private IInteractable currentInteractable;

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.Disable();
    }

    private void Update()
    {
        if (interactAction == null || interactAction.action == null)
            return;

        if (!interactAction.action.WasPressedThisFrame())
            return;

        Debug.Log("E pressed");

        if (currentInteractable == null)
        {
            Debug.Log("No item in range");
            return;
        }

        currentInteractable.Interact();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
            Debug.Log("Item in range: " + other.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable) &&
            interactable == currentInteractable)
        {
            currentInteractable = null;
            Debug.Log("Item out of range: " + other.name);
        }
    }
}