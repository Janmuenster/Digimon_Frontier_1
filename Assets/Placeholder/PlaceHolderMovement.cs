using UnityEngine;

public class PlaceHolderMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
   

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
    }

    void Update()
    {
        float moveHorizontal = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1f;
            spriteRenderer.flipX = true;
           
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1f;
            spriteRenderer.flipX = false;
        }

        Vector2 movement = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);
        rb.velocity = movement;
    }
}
