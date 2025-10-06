using UnityEngine;

public class LetterTile : MonoBehaviour
{
    [SerializeField] private string letter;
    [SerializeField] private bool isCollected = false;

    private Animator animator;
    private Collider2D tileCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        tileCollider = GetComponent<Collider2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.CompareTag("Player") && !isCollected)
        {

            if (collision.contacts[0].normal.y < -0.5f)
            {
                CollectLetter();
            }
        }
    }

    private void CollectLetter()
    {
        isCollected = true;

        // Disable collider so player can't collect it again
        if (tileCollider != null)
        {
            tileCollider.enabled = false;
        }

        // Trigger collection animation
        if (animator != null)
        {
            animator.SetTrigger("Collect");
        }

        // Notify the puzzle manager about the collected letter
        PuzzleManager.Instance?.OnLetterCollected(letter);

        // Destroy the tile after animation
        Destroy(gameObject, 0.5f);
    }

    public string GetLetter()
    {
        return letter;
    }
}