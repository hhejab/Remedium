using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteraction : MonoBehaviour
{
    private IInteractable currentInteractable;

    public void OnInteract(InputValue value)
    {
        if (!value.isPressed) return;

        Debug.Log("Interact pressed");

        if (currentInteractable != null)
        {
            Debug.Log("Calling interact on: " + ((MonoBehaviour)currentInteractable).name);
            currentInteractable.Interact();
        }
        else
        {
            Debug.Log("No interactable found");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Entered trigger: " + other.name);

        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null)
        {
            currentInteractable = interactable;
            Debug.Log("Interactable found: " + other.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log("Exited trigger: " + other.name);

        IInteractable interactable = other.GetComponent<IInteractable>();
        if (interactable != null && currentInteractable == interactable)
        {
            currentInteractable = null;
        }
    }
}