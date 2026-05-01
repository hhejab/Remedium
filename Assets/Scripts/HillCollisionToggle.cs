using UnityEngine;
using UnityEngine.Tilemaps;

public class HillToggle : MonoBehaviour
{
    public GameObject hillUpCollision;
    public GameObject hillAboveCollision;
    public TilemapRenderer hillAboveRenderer;

    public bool playerIsGoingUp; // TRUE = stairs up, FALSE = stairs down

    private void Start()
    {
        ApplyState(false); // default = player down
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        ApplyState(playerIsGoingUp);
    }

    void ApplyState(bool isUp)
    {
        if (isUp)
        {
            // PLAYER WENT UP
            hillAboveRenderer.sortingLayerName = "Ground";

            hillUpCollision.SetActive(true);
            hillAboveCollision.SetActive(false);
        }
        else
        {
            // PLAYER DOWN (DEFAULT)
            hillAboveRenderer.sortingLayerName = "AbovePlayer";

            hillUpCollision.SetActive(false);
            hillAboveCollision.SetActive(true);
        }

        Debug.Log("=== HILL STATE ===");
        Debug.Log("PlayerUp: " + isUp);
        Debug.Log("Sorting: " + hillAboveRenderer.sortingLayerName);
        Debug.Log("UpCollision: " + hillUpCollision.activeSelf);
        Debug.Log("AboveCollision: " + hillAboveCollision.activeSelf);
    }
}