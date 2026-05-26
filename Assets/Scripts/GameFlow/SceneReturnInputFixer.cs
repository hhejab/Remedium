using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneReturnInputFixer : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FixPlayer();
    }

    private void FixPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        PlayerInput input = player.GetComponent<PlayerInput>();

        if (input != null)
        {
            input.ActivateInput();

            if (input.actions != null)
            {
                input.actions.Enable();

                // re-enable BOTH maps
                var playerMap = input.actions.FindActionMap("Player");
                var uiMap = input.actions.FindActionMap("UI");

                if (playerMap != null)
                    playerMap.Enable();

                if (uiMap != null)
                    uiMap.Enable();
            }
        }

        PlayerCombat combat = player.GetComponent<PlayerCombat>();
        if (combat != null)
        {
            combat.enabled = false;
            combat.enabled = true;
        }

        Movement movement = player.GetComponent<Movement>();
        if (movement != null)
        {
            movement.enabled = false;
            movement.enabled = true;
        }
    }
}