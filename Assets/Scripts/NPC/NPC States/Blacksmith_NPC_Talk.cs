using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Blacksmith_NPC_Talk : MonoBehaviour
{
    [Header("Player")]
    public Transform player;
    public float interactionDistance = 1.5f;

    [Header("Upgrade UI")]
    public PlayerUpgradeManager upgradeManager;

    [Header("Interaction Bubble")]
    public GameObject interactionObject;
    public Animator interactionAnim;
    public float closeAnimationTime = 0.25f;

    private Animator npcAnim;
    private Rigidbody2D rb;

    private bool playerInRange;
    private bool isTalking;
    private Coroutine hideRoutine;

    private void Awake()
    {
        npcAnim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (interactionObject != null)
            interactionObject.SetActive(false);
    }

    private void Start()
    {
        FindPersistentPlayer();

        if (npcAnim != null)
            npcAnim.SetBool("isTalking", false);
    }

    private void FindPersistentPlayer()
    {
        if (player != null) return;

        GameObject p = GameObject.FindGameObjectWithTag("Player");

        if (p != null)
            player = p.transform;
        else
            Debug.LogError("Blacksmith cannot find Player. Make sure Player tag is Player.");
    }

    private void Update()
    {
        if (player == null)
        {
            FindPersistentPlayer();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactionDistance && !playerInRange)
        {
            playerInRange = true;
            ShowInteraction();
        }
        else if (distance > interactionDistance && playerInRange)
        {
            playerInRange = false;
            HideInteraction();

            if (isTalking)
                StopTalking();
        }

        if (playerInRange && InteractPressed())
        {
            StartTalking();
        }
    }

    private bool InteractPressed()
    {
        return Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
    }

    private void ShowInteraction()
    {
        if (hideRoutine != null)
        {
            StopCoroutine(hideRoutine);
            hideRoutine = null;
        }

        if (interactionObject != null)
            interactionObject.SetActive(true);

        if (interactionAnim != null)
            interactionAnim.Play("Open", 0, 0f);
    }

    private void HideInteraction()
    {
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideInteractionRoutine());
    }

    private IEnumerator HideInteractionRoutine()
    {
        if (interactionAnim != null)
            interactionAnim.Play("Close", 0, 0f);

        yield return new WaitForSeconds(closeAnimationTime);

        if (interactionObject != null)
            interactionObject.SetActive(false);

        hideRoutine = null;
    }

    private void StartTalking()
    {
        isTalking = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (npcAnim != null)
            npcAnim.SetBool("isTalking", true);

        if (upgradeManager != null)
            upgradeManager.OpenUpgradeCanvas();
        else
            Debug.LogError("UpgradeManager is missing on Blacksmith_NPC_Talk.");
    }

    public void StopTalking()
    {
        isTalking = false;

        if (npcAnim != null)
            npcAnim.SetBool("isTalking", false);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}