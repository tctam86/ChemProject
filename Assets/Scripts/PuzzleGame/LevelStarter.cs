using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    [Header("Difficulty Settings")]
    [SerializeField] private DifficultyLevel levelDifficulty = DifficultyLevel.Easy;
    [SerializeField] private float startDelay = 1f;

    void Start()
    {
        string savedDifficulty = PlayerPrefs.GetString("GameDifficulty", "Easy");

        switch (savedDifficulty)
        {
            case "Easy":
                levelDifficulty = DifficultyLevel.Easy;
                break;
            case "Normal":
                levelDifficulty = DifficultyLevel.Normal;
                break;
            case "Hard":
                levelDifficulty = DifficultyLevel.Hard;
                break;
            default:
                levelDifficulty = DifficultyLevel.Easy;
                break;
        }

        Debug.Log($"Level difficulty set to: {levelDifficulty} (from PlayerPrefs: {savedDifficulty})");
    }

    public void StartPuzzle()
    {
        if (QuestionBankManager.Instance != null)
        {
            Debug.Log($"Starting puzzle with difficulty: {levelDifficulty}");
            QuestionBankManager.Instance.StartPuzzle(levelDifficulty);
        }
        else
        {
            Debug.LogError("QuestionBankManager not found");
        }
    }
}