using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public GameObject Dialogue;
    public Text DialogueText;
    public string[] dialogue;
    private int index;

    public GameObject contButton;
    public float wordSpeed;
    public bool playerIsClose;
    
    // Neue Variablen für die Bewegung
    public Transform targetPosition;
    public float moveSpeed = 2f;
    private bool isMoving = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        DialogueText.text = "";
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E) && playerIsClose)
        {
            if(Dialogue.activeInHierarchy)
            {
                zeroText();
            }
            else
            {
                Dialogue.SetActive(true);
                StartCoroutine(Typing());
            }
        }

        if(DialogueText.text == dialogue[index])
        {
            contButton.SetActive(true);
        }

        // Bewegung des NPCs
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    public void zeroText()
    {
        DialogueText.text = "";
        index = 0;
        Dialogue.SetActive(false);
    }

    IEnumerator Typing()
    {
        foreach(char letter in dialogue[index].ToCharArray())
        {
            DialogueText.text += letter;
            yield return new WaitForSeconds(wordSpeed);
        }
    }

    public void NextLine()
    {
        contButton.SetActive(false);

        if(index < dialogue.Length - 1)
        {
            index++;
            DialogueText.text = "";
            StartCoroutine(Typing());
        }
        else
        {
            zeroText();
            StartMoving(); // Starte die Bewegung nach dem letzten Dialog
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Takuya"))
        {
            playerIsClose = true;
        }
        if (other.CompareTag("Zoe"))
        {
            playerIsClose = true;
        }
        if (other.CompareTag("Koji"))
        {
            playerIsClose = true;
        }
        if (other.CompareTag("Tommy"))
        {
            playerIsClose = true;
        }
        if (other.CompareTag("JP"))
        {
            playerIsClose = true;
        }
        if (other.CompareTag("Koiji"))
        {
            playerIsClose = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Takuya"))
        {
            playerIsClose = false;
            zeroText();
        }
        if (other.CompareTag("Zoe"))
        {
            playerIsClose = false;
            zeroText();
        }
        if (other.CompareTag("Koji"))
        {
            playerIsClose = false;
            zeroText();
        }
        if (other.CompareTag("Tommy"))
        {
            playerIsClose = false;
            zeroText();
        }
        if (other.CompareTag("JP"))
        {
            playerIsClose = false;
            zeroText();
        }
        if (other.CompareTag("Koiji"))
        {
            playerIsClose = false;
            zeroText();
        }
    }

    // Neue Methode zum Starten der Bewegung
    private void StartMoving()
    {
        if (targetPosition != null)
        {
            isMoving = true;
        }
    }

    // Neue Methode für die Bewegung zum Ziel
    private void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPosition.position, moveSpeed * Time.deltaTime);

        // Sprite-Richtung anpassen
        if (targetPosition.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false; // Nach rechts schauen
        }
        else if (targetPosition.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true; // Nach links schauen
        }

        // Bewegung stoppen, wenn das Ziel erreicht ist
        if (Vector2.Distance(transform.position, targetPosition.position) < 0.1f)
        {
            isMoving = false;
        }
    }
}
