using UnityEngine;

public class BossProgress : MonoBehaviour
{
    public static BossProgress Instance;

    public bool firstBossDefeated = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetFirstBossDefeated()
    {
        firstBossDefeated = true;
    }
}