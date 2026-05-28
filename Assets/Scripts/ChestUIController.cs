using UnityEngine;

public class ChestUIController : MonoBehaviour, IInteractable
{
    [Header("UI Reference")]
    public GameObject chestPanel; // Drag the ChestPanel here in the Inspector

    private void Start()
    {
        // Safely hide the UI when the game starts
        if (chestPanel != null)
        {
            chestPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("ChestPanel is not assigned on " + gameObject.name);
        }
    }

    public void Interact()
    {
        if (chestPanel == null) return;

        // This line tells you IF it's in the scene or just a file
        Debug.Log("I am trying to toggle an object named: " + chestPanel.name + ". Is it in the scene? " + chestPanel.scene.IsValid());

        chestPanel.SetActive(!chestPanel.activeSelf);
    }
}