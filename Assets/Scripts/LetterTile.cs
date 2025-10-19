using UnityEngine;

public class LetterTile : MonoBehaviour
{
    [SerializeField] private string letter;
    [SerializeField] private TMPro.TMP_Text letterLabel;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isCollected = false;

    private Animator animator;
    private Collider2D tileCollider;
    private Rigidbody2D rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        tileCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y < -0.5f)
        {
            CollectLetter();
        }
    }

    private void CollectLetter()
    {
        isCollected = true;

        Debug.Log($"🎯 Collected letter: {letter}");


        if (tileCollider != null)
        {
            tileCollider.enabled = false;
        }


        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0, 8f);
            rb.gravityScale = 2f;
        }


        if (animator != null)
        {
            animator.SetTrigger("Collect");
        }


        PuzzleManager.Instance?.OnLetterCollected(letter);


        Destroy(gameObject, 1.5f);
    }

    public string GetLetter() => letter;

    public void SetLetter(string newLetter)
    {
        letter = newLetter;
        if (letterLabel != null)
        {
            letterLabel.text = newLetter;
        }
    }

    public void SetSprite(Sprite newSprite)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = newSprite;
        }
    }
}
