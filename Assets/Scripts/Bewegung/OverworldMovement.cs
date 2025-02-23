using UnityEngine;

public class OverworldMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()

    {
        rb = GetComponent<Rigidbody2D>();

        // Deaktiviere das Skript, falls wir nicht in der Overworld sind
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Overworld")
        {
            this.enabled = false;
        }
    }

    void Update()
    {
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        rb.velocity = movement * moveSpeed;
    }

}
