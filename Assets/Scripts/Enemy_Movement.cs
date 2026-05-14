using UnityEngine;

public class Enemy_Movement : MonoBehaviour
{
    public float speed;
    private bool isChasing;

    private Rigidbody2D rb;
    private Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isChasing == true)
        {
            Vector2 direction = new Vector2(
                player.position.x - transform.position.x,
                player.position.y - transform.position.y
            ).normalized;

            rb.linearVelocity = direction * speed;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if(player == null)
            { 
                player = collision.transform;
            }
            isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            rb.linearVelocity = Vector2.zero;
            isChasing = false;
        }
    }
}
