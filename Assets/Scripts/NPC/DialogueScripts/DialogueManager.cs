using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI References")]
    public CanvasGroup canvasGroup;
    public Image portrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;

    public bool isDialogueActive;

    private DialogueSO currentDialogue;
    private int dialogueIndex;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        HideDialogueUI();
    }

    public void StartDialogue(DialogueSO dialogueSO)
    {
        if (dialogueSO == null)
        {
            Debug.LogWarning("DialogueSO is missing.");
            return;
        }

        if (dialogueSO.lines == null || dialogueSO.lines.Length == 0)
        {
            Debug.LogWarning("DialogueSO has no lines.");
            return;
        }

        currentDialogue = dialogueSO;
        dialogueIndex = 0;
        isDialogueActive = true;

        ShowDialogue();
    }

    public void AdvanceDialogue()
    {
        if (!isDialogueActive || currentDialogue == null)
            return;

        if (dialogueIndex >= currentDialogue.lines.Length)
        {
            CloseDialogue();
            return;
        }

        ShowDialogue();
    }

    private void ShowDialogue()
    {
        if (currentDialogue == null) return;

        if (dialogueIndex >= currentDialogue.lines.Length)
        {
            CloseDialogue();
            return;
        }

        DialogueLine line = currentDialogue.lines[dialogueIndex];

        if (line.speaker == null)
        {
            Debug.LogWarning("Speaker is missing in dialogue line " + dialogueIndex);
            return;
        }

        if (portrait != null)
            portrait.sprite = line.speaker.portrait;

        if (actorName != null)
            actorName.text = line.speaker.actorName;

        if (dialogueText != null)
            dialogueText.text = line.text;

        ShowDialogueUI();

        dialogueIndex++;
    }

    public void CloseDialogue()
    {
        isDialogueActive = false;
        dialogueIndex = 0;
        currentDialogue = null;

        HideDialogueUI();
    }

    private void ShowDialogueUI()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideDialogueUI()
    {
        if (canvasGroup == null) return;

        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}