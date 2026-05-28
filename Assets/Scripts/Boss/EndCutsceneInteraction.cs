using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class EndCutsceneInteraction : MonoBehaviour
{
    public Transform player;
    public float interactionDistance = 1.5f;

    public GameObject interactionObject;
    public Animator interactionAnim;

    public InputActionReference interactAction;

    public string endCutsceneSceneName = "EndCutscene";

    private bool playerInRange;

  private void Awake()
{
    if (player == null)
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
    }

    if (interactionObject != null)
        interactionObject.SetActive(false);
}

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactionDistance && !playerInRange)
        {
            playerInRange = true;
            ShowInteraction();
        }

        if (distance > interactionDistance && playerInRange)
        {
            playerInRange = false;
            HideInteraction();
        }

        if (playerInRange && interactAction.action.WasPressedThisFrame())
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(endCutsceneSceneName);
        }
    }

    private void ShowInteraction()
    {
        if (interactionObject != null)
            interactionObject.SetActive(true);

        if (interactionAnim != null)
            interactionAnim.Play("Open");
    }

    private void HideInteraction()
    {
        if (interactionAnim != null)
            interactionAnim.Play("Close");

        if (interactionObject != null)
            interactionObject.SetActive(false);
    }
}