using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            currentInteractable = interactable;
            Debug.Log("Interactable found: " + other.name);
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

 public void OnInteract(InputValue value)
{
    if (!value.isPressed) return;

    Debug.Log("E pressed");

    if (currentInteractable != null)
        currentInteractable.Interact();
}
}