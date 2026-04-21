using UnityEngine;
using UnityEngine.SceneManagement;

public class BootLoader : MonoBehaviour
{   
    [SerializeField] private Object persistentScene;
    [SerializeField] private Object firstGameplayScene;

    private void Start()
    {
        if (persistentScene != null)
        {
            string persistentName = persistentScene.name;

            if (!SceneManager.GetSceneByName(persistentName).isLoaded)
                SceneManager.LoadScene(persistentName, LoadSceneMode.Additive);
        }

        if (firstGameplayScene != null)
        {
            string gameplayName = firstGameplayScene.name;

            if (!SceneManager.GetSceneByName(gameplayName).isLoaded)
                SceneManager.LoadScene(gameplayName, LoadSceneMode.Additive);
        }
    }

    // Update is called once per frame
}
