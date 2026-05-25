using UnityEngine;

public class PersistentGameSystems : MonoBehaviour
{
    public static PersistentGameSystems Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}