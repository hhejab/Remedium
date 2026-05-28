using UnityEngine;

public class HidePersistentObjectsInMenu : MonoBehaviour
{
    private void Start()
    {
        GameObject gameSystems = GameObject.Find("GameSystems");
        if (gameSystems != null)
            Destroy(gameSystems);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            Destroy(player);
    }
}