using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUpgradeManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject upgradeCanvas;

    [Header("Input")]
    public InputActionReference interactAction;

    [Header("CP Cost")]
    public int upgradeCostCP = 1;

    [Header("Animator Controllers")]
    public RuntimeAnimatorController playerLevel1Controller;
    public RuntimeAnimatorController playerLevel2Controller;

    [Header("Optional Scene Reference")]
    public SkillBookManager skillBookManager;

    [Header("Upgrade Buffs")]
    public float attackDamagePercentBonus = 10f;
    public float attackSpeedBonus = 0.25f;
    public float critChanceBonus = 5f;
    public int maxHealthBonus = 25;
    public int defenseBonus = 2;
    public float walkSpeedBonus = 0.2f;

    private GameObject playerObject;
    private Animator playerAnimator;
    private SpriteRenderer playerSpriteRenderer;
    private PlayerStats playerStats;
    private PlayerHealth playerHealth;
    private PlayerCPWallet cpWallet;

    private bool upgraded = false;
    private bool canvasOpen = false;
    private bool ignoreFirstInteractPress = false;

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.Disable();
    }

    private void Start()
    {
        FindPlayerReferences();

        if (upgradeCanvas != null)
            upgradeCanvas.SetActive(false);
    }

    private void Update()
    {
        if (!canvasOpen) return;

        if (ignoreFirstInteractPress)
        {
            if (interactAction != null && !interactAction.action.IsPressed())
                ignoreFirstInteractPress = false;

            return;
        }

        if (interactAction != null && interactAction.action.WasPressedThisFrame())
            CloseUpgradeCanvas();
    }

    private void FindPlayerReferences()
    {
        if (playerObject == null)
            playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("PlayerUpgradeManager cannot find Player. Make sure Player tag is Player.");
            return;
        }

        if (playerAnimator == null)
            playerAnimator = playerObject.GetComponentInChildren<Animator>();

        if (playerSpriteRenderer == null)
            playerSpriteRenderer = playerObject.GetComponentInChildren<SpriteRenderer>();

        if (playerStats == null)
            playerStats = playerObject.GetComponent<PlayerStats>();

        if (playerHealth == null)
            playerHealth = playerObject.GetComponent<PlayerHealth>();

        if (cpWallet == null)
            cpWallet = playerObject.GetComponent<PlayerCPWallet>();

        if (skillBookManager == null)
            skillBookManager = FindFirstObjectByType<SkillBookManager>();
    }

    public void OpenUpgradeCanvas()
    {
        FindPlayerReferences();

        if (upgradeCanvas != null)
            upgradeCanvas.SetActive(true);

        canvasOpen = true;
        ignoreFirstInteractPress = true;

        Time.timeScale = 0f;

        if (CursorManager.Instance != null)
            CursorManager.Instance.OpenUI();
    }

    public void CloseUpgradeCanvas()
    {
        if (upgradeCanvas != null)
            upgradeCanvas.SetActive(false);

        canvasOpen = false;
        Time.timeScale = 1f;

        if (CursorManager.Instance != null)
            CursorManager.Instance.CloseUI();
    }

    public void UpgradePlayer()
    {
        FindPlayerReferences();

        Debug.Log("Upgrade button clicked.");

        if (upgraded)
        {
            Debug.Log("Player is already upgraded.");
            CloseUpgradeCanvas();
            return;
        }

        if (cpWallet == null || !cpWallet.SpendCP(upgradeCostCP))
        {
            Debug.Log("Not enough CP to upgrade.");
            return;
        }

        upgraded = true;

        if (playerAnimator != null && playerLevel2Controller != null)
        {
            playerAnimator.runtimeAnimatorController = playerLevel2Controller;
            playerAnimator.SetBool("isMoving", false);
            playerAnimator.SetBool("isRunning", false);

            Debug.Log("Player animator changed to Level 2.");
        }
        else
        {
            Debug.LogError("Player Animator or Player Level 2 Controller is missing.");
        }

        if (playerSpriteRenderer != null)
        {
            playerSpriteRenderer.enabled = true;
            playerSpriteRenderer.color = Color.white;
        }

        if (playerStats != null)
        {
            playerStats.attackDamagePercent += attackDamagePercentBonus;
            playerStats.attackSpeed += attackSpeedBonus;
            playerStats.critChance += critChanceBonus;
            playerStats.critChance = Mathf.Clamp(playerStats.critChance, 0f, 100f);
            playerStats.defense += defenseBonus;
            playerStats.walkSpeedBonus += walkSpeedBonus;
        }

        if (playerHealth != null)
            playerHealth.AddMaxHealthAndHeal(maxHealthBonus);

        if (skillBookManager != null)
            skillBookManager.RefreshAll();

        PlayerLevel playerLevel = playerObject.GetComponent<PlayerLevel>();
        if (playerLevel != null)
            playerLevel.SetLevel(2);

        CloseUpgradeCanvas();
    }
}