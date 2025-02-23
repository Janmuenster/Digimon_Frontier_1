using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    public Animator animator;
    private bool isMoving = false;
    private Vector2 movement;

    public PlayerStats playerStats; // Referenz zum PlayerStats-Skript

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerStats = GetComponent<PlayerStats>(); // Holen Sie sich die Referenz zum PlayerStats-Skript
        LoadPlayerData(); // Laden der Spielerdaten beim Start
        SavePlayerData();
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        float moveHorizontal = 0f;
        float moveVertical = 0f;

        if (Input.GetKey(KeyCode.A))
        {
            moveHorizontal = -1f;
            spriteRenderer.flipX = true;
            SetMovingState(true);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            moveHorizontal = 1f;
            spriteRenderer.flipX = false;
            SetMovingState(true);
        }

        if (Input.GetKey(KeyCode.W))
        {
            moveVertical = 1f;
            SetMovingState(true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            moveVertical = -1f;
            SetMovingState(true);
        }

        if (moveHorizontal == 0f && moveVertical == 0f)
        {
            SetMovingState(false);
        }

        movement = new Vector2(moveHorizontal * moveSpeed, moveVertical * moveSpeed);
        rb.velocity = movement;

        // Aktualisieren Sie die Position im PlayerStats-Skript
        playerStats.positionX = transform.position.x;
        playerStats.positionY = transform.position.y;
    }

    private void SetMovingState(bool moving)
    {
        if (moving != isMoving)
        {
            isMoving = moving;
            if (moving)
            {
                animator.SetTrigger("StartMoving");
            }
            else
            {
                animator.SetTrigger("StopMoving");
            }
        }
    }

    public void SavePlayerData()
    {
        Debug.Log(" Speichere Spielerposition vor Kampf: " + transform.position);
        playerStats.positionX = transform.position.x;
        playerStats.positionY = transform.position.y;
        playerStats.overworldSceneName = SceneManager.GetActiveScene().name;
        saveManager.SavePlayer(playerStats);
    }

    public void LoadPlayerData()
    {
        PlayerData data = saveManager.LoadPlayer();
        if (data != null)
        {
            playerStats.level = data.level;
            playerStats.currentHealth = data.currentHealth;
            playerStats.experience = data.experience;
            transform.position = new Vector3(data.positionX, data.positionY, 0);
            // Überprüfen Sie, ob die geladene Szene mit der gespeicherten übereinstimmt
            if (SceneManager.GetActiveScene().name != data.overworldSceneName)
            {
                SceneManager.LoadScene(data.overworldSceneName);
            }
        }
    }

    void OnDisable()
    {
        SavePlayerData(); // Speichern der Daten, wenn das Objekt deaktiviert wird
    }
}
