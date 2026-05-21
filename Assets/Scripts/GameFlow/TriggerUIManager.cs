using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TriggerUIManager : MonoBehaviour
{
    public static TriggerUIManager Instance;

    public GameObject panel;
    public TMP_Text messageText;
    public Button yesButton;
    public Button noButton;

    private Action yesAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (panel != null)
            panel.SetActive(false);

        if (yesButton != null)
        {
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(Yes);
        }

        if (noButton != null)
        {
            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(No);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void Show(string message, Action onYes)
    {
        yesAction = onYes;

        if (messageText != null)
            messageText.text = message;

        if (panel != null)
            panel.SetActive(true);

        Time.timeScale = 0f;
    }

    private void Yes()
    {
        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = 1f;
        yesAction?.Invoke();
    }

    private void No()
    {
        if (panel != null)
            panel.SetActive(false);

        Time.timeScale = 1f;
        yesAction = null;
    }
}