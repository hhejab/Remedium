using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class CameraBoundaryLoader : MonoBehaviour
{
    public CinemachineConfiner2D confiner;

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
        GameObject boundary = GameObject.Find("CameraBoundary");

        if (boundary == null)
        {
            Debug.LogWarning("No CameraBoundary found in scene: " + scene.name);
            return;
        }

        PolygonCollider2D polygon = boundary.GetComponent<PolygonCollider2D>();

        if (polygon == null)
        {
            Debug.LogWarning("CameraBoundary needs PolygonCollider2D");
            return;
        }

        confiner.BoundingShape2D = polygon;
        confiner.InvalidateBoundingShapeCache();
    }
}