using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputResetter : MonoBehaviour
{
    private PlayerInput playerInput;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        ResetInput();
    }

    private void Update()
    {
        ResetInput();
    }

    private void ResetInput()
    {
        if (playerInput == null) return;

        playerInput.ActivateInput();

        if (playerInput.actions == null) return;

        playerInput.actions.Enable();

        var playerMap = playerInput.actions.FindActionMap("Player");
        if (playerMap != null && !playerMap.enabled)
            playerMap.Enable();

        var uiMap = playerInput.actions.FindActionMap("UI");
        if (uiMap != null && !uiMap.enabled)
            uiMap.Enable();
    }
}