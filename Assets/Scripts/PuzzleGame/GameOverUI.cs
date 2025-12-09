using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour
{
    public static GameOverUI Instance { get; private set; }
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text motivationalText;
    [SerializeField] private TMP_Text finalScoreText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        if (motivationalText != null)
            motivationalText.text = MotivationalBank.Instance.GetRandomMessage();

        // Display final score
        if (finalScoreText != null && PuzzleManager.Instance != null)
        {
            int finalScore = PuzzleManager.Instance.GetCurrentScore();
            finalScoreText.text = $"Final Score: {finalScore:N0}";
        }
        else if (finalScoreText == null)
        {
            Debug.LogWarning("Final Score Text is not assigned in the inspector!");
        }
        else if (PuzzleManager.Instance == null)
        {
            Debug.LogWarning("PuzzleManager.Instance is null!");
        }

        Time.timeScale = 0f;
    }


    public void TryAgain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
