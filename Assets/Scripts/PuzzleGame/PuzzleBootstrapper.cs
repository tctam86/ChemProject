using UnityEngine;

public class PuzzleBootstrapper : MonoBehaviour
{
    public DifficultyLevel difficulty = DifficultyLevel.Easy;

    void Start()
    {
        if (QuestionBankManager.Instance != null)
        {
            QuestionBankManager.Instance.StartPuzzle(difficulty);
        }
        else
        {
            Debug.LogError("QuestionBankManager.Instance is null. Make sure a QuestionBankManager exists in the scene.");
        }
    }
}
