using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerInteraction : MonoBehaviour
{
    public InputActionReference interactAction; // Drag your 'Interact' action here
    
    // List to track all interactables currently in range
    private List<IInteractable> interactablesInRange = new List<IInteractable>();

    private void OnEnable() => interactAction.action.Enable();
    private void OnDisable() => interactAction.action.Disable();

    private void Update()
    {
        // 1. Check if E was pressed
        if (interactAction.action.WasPressedThisFrame())
        {
            // 2. Filter out any objects that were destroyed but are still in the list
            interactablesInRange.RemoveAll(i => i == null || (i is MonoBehaviour mb && mb == null));

            // 3. If we have items in range, interact with the last one added
            if (interactablesInRange.Count > 0)
            {
                IInteractable target = interactablesInRange[interactablesInRange.Count - 1];
                target.Interact();
                Debug.Log("Interacting with object...");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            if (!interactablesInRange.Contains(interactable))
            {
                interactablesInRange.Add(interactable);
                Debug.Log("Range Entered: " + other.name);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IInteractable>(out var interactable))
        {
            interactablesInRange.Remove(interactable);
            Debug.Log("Range Exited: " + other.name);
        }
    }
}