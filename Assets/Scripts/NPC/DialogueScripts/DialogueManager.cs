using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public CanvasGroup canvasGroup;

    public Image portrait;
    public TMP_Text actorName;
    public TMP_Text dialogueText;

    [Header("Buttons")]
    public Button nextButton;

    public Button option1Button;
    public Button option2Button;

    public TMP_Text option1Text;
    public TMP_Text option2Text;

    private DialogueSO currentDialogue;
    private int dialogueIndex;

    public bool isDialogueActive;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        HideDialogueUI();
    }

    public void StartDialogue(DialogueSO dialogueSO)
    {
        if (dialogueSO == null)
            return;

        currentDialogue = dialogueSO;

        dialogueIndex = 0;

        isDialogueActive = true;

        ShowDialogue();
    }

    public void AdvanceDialogue()
    {
        if (!isDialogueActive)
            return;

        dialogueIndex++;

        ShowDialogue();
    }

    private void ShowDialogue()
    {
        if (currentDialogue == null)
            return;

        if (dialogueIndex >= currentDialogue.lines.Length)
        {
            ShowOptionsOrClose();
            return;
        }

        DialogueLine line = currentDialogue.lines[dialogueIndex];

        if (line.speaker != null)
        {
            portrait.sprite = line.speaker.portrait;
            actorName.text = line.speaker.actorName;
        }

        dialogueText.text = line.text;

        ShowDialogueUI();

        if (dialogueIndex < currentDialogue.lines.Length - 1)
        {
            nextButton.gameObject.SetActive(true);

            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);
        }
        else
        {
            nextButton.gameObject.SetActive(false);

            ShowOptionsOrClose();
        }
    }

    private void ShowOptionsOrClose()
    {
        if (currentDialogue.options.Length >= 2)
        {
            option1Button.gameObject.SetActive(true);
            option2Button.gameObject.SetActive(true);

            DialogueOption option1 = currentDialogue.options[0];
            DialogueOption option2 = currentDialogue.options[1];

            option1Text.text = option1.optionText;
            option2Text.text = option2.optionText;

            option1Button.onClick.RemoveAllListeners();
            option2Button.onClick.RemoveAllListeners();

            option1Button.onClick.AddListener(() =>
            {
                StartDialogue(option1.nextDialogue);
            });

            option2Button.onClick.AddListener(() =>
            {
                StartDialogue(option2.nextDialogue);
            });
        }
        else
        {
            option1Button.gameObject.SetActive(false);
            option2Button.gameObject.SetActive(false);

            Invoke(nameof(CloseDialogue), 1f);
        }
    }

    public void CloseDialogue()
    {
        isDialogueActive = false;

        currentDialogue = null;

        dialogueIndex = 0;

        HideDialogueUI();
    }

    private void ShowDialogueUI()
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    private void HideDialogueUI()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        nextButton.gameObject.SetActive(false);

        option1Button.gameObject.SetActive(false);
        option2Button.gameObject.SetActive(false);
    }
}