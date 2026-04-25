using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour, IInteractable
{
    [SerializeField] private UnityEditor.SceneAsset sceneAsset; // drag scene here

    private string sceneToLoad;

    private void OnValidate()
    {
        if (sceneAsset != null)
        {
            sceneToLoad = sceneAsset.name;
        }
    }

    public void Interact()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}