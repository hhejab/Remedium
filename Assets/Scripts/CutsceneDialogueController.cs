using UnityEngine;
using TMPro;

public class CutsceneDialogueController : MonoBehaviour
{
    public GameObject dialogueCanvas;
    public TMP_Text actorName;
    public TMP_Text dialogueText;

    public string speaker;
    [TextArea] public string line;

    private void Start()
    {
        dialogueCanvas.SetActive(false);
    }

    public void ShowDialogue()
    {
        dialogueCanvas.SetActive(true);
        actorName.text = speaker;
        dialogueText.text = line;
    }

    public void HideDialogue()
    {
        dialogueCanvas.SetActive(false);
    }
}