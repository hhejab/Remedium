using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SkillBookUI : MonoBehaviour
{
    [Header("Input")]
    public InputActionReference skillBookAction;

    [Header("Main UI")]
    public GameObject skillBookPanel;
    public Animator bookAnimator;

    [Header("Fade UI")]
    public CanvasGroup skillPageCanvasGroup;
    public CanvasGroup statsPageCanvasGroup;

    [Header("Timing")]
    public float openAnimationTime = 0.8f;
    public float contentFadeTime = 0.25f;
    public float closeAnimationTime = 0.8f;

    private bool isOpen;
    private bool isBusy;
    private Coroutine currentRoutine;

    private void Awake()
    {
        if (skillBookPanel != null)
            skillBookPanel.SetActive(false);

        if (bookAnimator != null)
            bookAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

        HideContentInstant();
    }

    private void OnEnable()
    {
        if (skillBookAction != null)
            skillBookAction.action.Enable();
    }

    private void OnDisable()
    {
        if (skillBookAction != null)
            skillBookAction.action.Disable();
    }

    private void Update()
    {
        if (skillBookAction != null && skillBookAction.action.WasPressedThisFrame())
        {
            ToggleBook();
        }
    }

    public void ToggleBook()
    {
        if (isBusy) return;

        if (isOpen)
            CloseBook();
        else
            OpenBook();
    }

    public void OpenBook()
    {
        if (isBusy) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(OpenBookRoutine());
    }

    public void CloseBook()
    {
        if (isBusy) return;
        if (!isOpen) return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(CloseBookRoutine());
    }

    private IEnumerator OpenBookRoutine()
{
    isBusy = true;
    isOpen = true;

    Time.timeScale = 0f;

    if (skillBookPanel != null)
        skillBookPanel.SetActive(true);

    HideContentInstant();

    if (bookAnimator != null)
        bookAnimator.Play("OpenBook", 0, 0f);

    // Wait 1 frame so Animator updates
    yield return null;

    // Wait until Animator reaches IdleBook
    while (bookAnimator != null && !bookAnimator.GetCurrentAnimatorStateInfo(0).IsName("IdleBook"))
    {
        yield return null;
    }

    // Now IdleBook has started, fade in content
    yield return FadeContent(0f, 1f, contentFadeTime);

    SetContentInteractable(true);

    isBusy = false;
}

    private IEnumerator CloseBookRoutine()
    {
        isBusy = true;

        // First fade out SkillPage + StatsPage
        SetContentInteractable(false);
        yield return FadeContent(1f, 0f, contentFadeTime);

        // Then play CloseBook animation
        if (bookAnimator != null)
            bookAnimator.SetTrigger("Close");

        yield return new WaitForSecondsRealtime(closeAnimationTime);

        if (skillBookPanel != null)
            skillBookPanel.SetActive(false);

        Time.timeScale = 1f;

        isOpen = false;
        isBusy = false;
    }

    private IEnumerator FadeContent(float from, float to, float duration)
    {
        float timer = 0f;

        SetCanvasGroupAlpha(skillPageCanvasGroup, from);
        SetCanvasGroupAlpha(statsPageCanvasGroup, from);

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            float t = timer / duration;
            float alpha = Mathf.Lerp(from, to, t);

            SetCanvasGroupAlpha(skillPageCanvasGroup, alpha);
            SetCanvasGroupAlpha(statsPageCanvasGroup, alpha);

            yield return null;
        }

        SetCanvasGroupAlpha(skillPageCanvasGroup, to);
        SetCanvasGroupAlpha(statsPageCanvasGroup, to);
    }

    private void HideContentInstant()
    {
        SetCanvasGroupAlpha(skillPageCanvasGroup, 0f);
        SetCanvasGroupAlpha(statsPageCanvasGroup, 0f);
        SetContentInteractable(false);
    }

    private void SetCanvasGroupAlpha(CanvasGroup canvasGroup, float alpha)
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = alpha;
    }

    private void SetContentInteractable(bool value)
    {
        SetCanvasGroupInteractable(skillPageCanvasGroup, value);
        SetCanvasGroupInteractable(statsPageCanvasGroup, value);
    }

    private void SetCanvasGroupInteractable(CanvasGroup canvasGroup, bool value)
    {
        if (canvasGroup == null) return;

        canvasGroup.interactable = value;
        canvasGroup.blocksRaycasts = value;
    }
}