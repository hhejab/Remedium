using UnityEngine;

public class HideGameSystemsInCutscene : MonoBehaviour
{
    private void Start()
    {
        GameObject gameSystems = GameObject.Find("GameSystems");
        if (gameSystems != null)
            gameSystems.SetActive(false);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            player.SetActive(false);
    }
}
